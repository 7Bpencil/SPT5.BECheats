# Notes on client modding obstacles and solutions

## Types do not implement interfaces anymore

Interfaces are stripped from types, but `Il2CppInterop` has mapping of which types implement which interfaces. We just have to manually cast instances:

```cs
public void BaseGameMethod(IContainer container) {}

Slot slot = // got it from somewhere

// in 4.1 Slot implements IContainer, casting is done implicitly
BaseGameMethod(slot);

// in 5.0 Slot inherits Il2CppObjectBase, but doesn't implement any interfaces,
// so we have to do explicit casting via Il2CppObjectBase.TryCast method
BaseGameMethod(slot.TryCast<IContainer>());
```
