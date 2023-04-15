[![openupm](https://img.shields.io/npm/v/net.mirzipan.infusion?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/net.mirzipan.infusion/) ![GitHub](https://img.shields.io/github/license/Mirzipan/Mirzipan.Infusion)

# Mirzipan.Infusion
Basic dependency injection

## Inject Attribute
This attribute marks any field or property as injectable. 
When an instance of an object with injectable members is used in injection container, all known instances of desired types will be injected automagically. 

```csharp
public class AClass 
{
    [Inject("my-favorite-bclass-instance")]
    private readonly BClass _injectableField;
    [Inject]
    private readonly CClass InjectableProperty { get; }
    [Inject("my-favorite-bclass-instance", requireInstance = true)]
    private readonly AStruct _injectableField;
    
    [Inject]
    private void Init()
    {
    }
    
    [Inject]
    private void Init(CClass someCClass)
    {
    }
}
```

Inject can use both, an unnamed instance or a named instance.

## Injection Container

### Hierarchy

```csharp
string Name { get; }
IInjectionContainer Parent { get; }
IReadOnlyList<IInjectionContainer> Children { get; }
IInjectionContainer CreateChild(string name);
```
Containers can exist in hierarchy. 
If child container has no binding for what you are trying to resolve, it will try resolving the instance via parent.

### Inject

```csharp
void Inject(object instance)
```
Goes through all members marked with `[Inject]` and injects any registered instances of member types. It will try and create instances if non were found.

```csharp
void InjectAll()
```
Goes through all registered instances and calls `Inject()` on them.

### Resolve

```csharp
T Resolve<T>(string identifier = null, bool requireInstance = false, object[] args = null) where T : class
object Resolve(Type baseType, string identifier = null, bool requireInstance = false, object[] constructorArgs = null)        
```
Get the registered instance of the Type.
* `identifier` - optional name the instance was registered with.
* `requireInstance` - optional, if true, it will only resolve the instance if one was registered, otherwise it will try to create one.
* `args` - optional constructor arguments for when no instance was registered and one needs to be created.

```csharp
IEnumerable<T> ResolveAll<T>()
IEnumerable<object> ResolveAll(Type type)
```
Get all registered instances of the Type.

### Instantiate

```csharp
T Instantiate<T>(object[] constructorArgs = null)
object Instantiate(Type type, object[] constructorArgs = null)
```
Returns a fresh instance of the Type and injects it.
* `constructorArgs` - optional constructor arguments

### Bind

```csharp        
void BindInstance(object instance, string identified = null);
void BindInstance<T>(T instance);
void BindInstance<T>(T instance, bool injectNow);
void BindInstance<T>(T instance, string identifier, bool injectNow = true);
void BindInstance(Type baseType, object instance, string identifier = null, bool injectNow = true);
void BindLazy<T>(string identifier = null);
void BindLazy(Type type, string identifier = null);
void BindFactory<TBase, TConcrete>(string identifier = null) where TConcrete : TBase;
void BindFactory(Type baseType, Type concreteType, string identifier = null);
void BindFunction<T>(Func<T> factory, string identifier = null);
```
Register a specific instance with the Type.
* `indentifier` - optional identifier for the instance.
* `injectNow` - optional, if true, this instance will be injected.

### Unbind

```csharp
void Unbind<T>(string identifier = null)
void Unbind(Type forType, string identifier = null)
```
Unregisters the instance associated with the Type.
* `indentifier` - optional identifier for the instance.

### Has Bindings

```csharp
bool HasBinding<T>(string identifier = null)
bool HasBinding(Type type, string identifier = null)
```
Returns true if there is an instance registered with the Type.
* `indentifier` - optional identifier for the instance.
