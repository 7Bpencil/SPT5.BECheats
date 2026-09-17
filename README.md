# Notes on client modding obstacles and solutions

## Target framework changed

Move to NET 6.0

## Dependencies changed place

* `EscapeFromTarkov_Data\Managed` -> `BepInEx\interop`
* `BepInEx\plugins\spt` -> `BepInEx\plugins\sptushonka`

## Dependencies changed name

* `spt-reflection.dll` -> `SPTushonka.Reflection.dll`
* `BepInEx.dll` -> `BepInEx.Core.dll`

## New dependencies

* `BepInEx.Unity.IL2CPP.dll`
* `Il2Cppmscorlib.dll`
* `Il2CppInterop.Runtime.dll`

## Almost all private became public

* Patches that access private fields via three underscores will crash, replace with direct access from __instance
* Reflection used to access private fields will crash too, replace with direct access

## Generic AssetBundle.LoadAsset doesnt exist

Add this wrapper to your project

```cs
public static class AssetBundleExtensions
{
  public static T LoadAsset<T>(this AssetBundle bundle, string path) where T : class
  {
    return bundle.LoadAsset(path, Il2CppInterop.Runtime.Il2CppType.Of<T>()).TryCast<T>();
  }
}
```

## Custom MonoBehaviour cannot have custom types in method parameters

We have to register every custom MonoBehaviour in mod assembly otherwise it wont work:

```cs
[BepInPlugin("guid", "name", "version")]
public class Plugin : BasePlugin
{
  public override void Load()
  {
    ClassInjector.RegisterTypeInIl2Cpp<MyMono>();
    // ...
  }
}

```

But some will fail to register, in this example MyMono fails because DoWork has parameter data of custom type MyClass:

```cs
public class MyClass { }

public class MyMono : MonoBehaviour
{
  public void DoWork(MyClass data, Transform root, bool flag)
  {
    // ...
  }
}
```

Fix: move parameter to field

```cs
public class MyMono : MonoBehaviour
{
  public MyClass _DoWork_data;

  public void DoWork(Transform root, bool flag)
  {
    var data = _DoWork_data;
    _DoWork_data = null;
    // ...
  }
}
```

And set it before calling method:

```cs
public void UseMyMono(MyMono instance)
{
  // ...
  instance._DoWork_data = some_data;
  instance.DoWork(some_root, some_flag);
  // ...
}
```

## Converting delegates to Il2Cpp happy types

In 4.1:

```cs
public void DoWork(Action<int> callback)

public void User()
{
  DoWork(result => Logger.Log($"did the work: {result}"));
}
```

In 5.0 convert via Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<T>

```cs
using Il2CppInterop.Runtime;

public void User()
{
  DoWork(DelegateSupport.ConvertDelegate<Il2CppSystem.Action<int>>(result => Logger.Log($"did the work: {result}")));
}
```
