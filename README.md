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
