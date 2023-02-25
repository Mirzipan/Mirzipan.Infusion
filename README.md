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
}
```

Inject can use both, an unnamed instance or a named instance.

## Injection Container

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
void Bind<T>(T instance) where T : class
void Bind<T>(T instance, bool injectNow) where T : class
void Bind<T>(T instance, string identifier, bool injectNow = true) where T : class
void Bind(Type baseType, object instance, string identifier = null, bool injectNow = true)
```
Register a specific instance with the Type.
* `indentifier` - optional identifier for the instance.
* `injectNow` - optional, if true, this instance will be injected.

```csharp
void BindWithInterfaces<T>(T instance) where T : class
void BindWithInterfaces<T>(T instance, bool injectNow) where T : class
void BindWithInterfaces<T>(T instance, string identifier, bool injectNow = true) where T : class
void BindWithInterfaces(Type baseType, object instance, string identifier = null, bool injectNow = true)
```
Register a specific instance with the Type and all interfaces it implements.
* `indentifier` - optional identifier for the instance.
* `injectNow` - optional, if true, this instance will be injected.

### Unbind

```csharp
void Unbind<T>(string identifier = null)
void Unbind(Type forType, string identifier = null)
```
Unregisters the instance associated with the Type.
* `indentifier` - optional identifier for the instance.

```csharp
void UnbindInstances()
```
Unregisters all registered instances.

### Has Bindings

```csharp
bool HasBinding<T>()
bool HasBinding<T>(string identifier)
bool HasBinding(Type type)
bool HasBinding(Type type, string identifier)
```
Returns true if there is an instance registered with the Type.
* `indentifier` - optional identifier for the instance.
