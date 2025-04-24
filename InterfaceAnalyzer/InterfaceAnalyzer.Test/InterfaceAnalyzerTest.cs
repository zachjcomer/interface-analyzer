using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.NUnit.AnalyzerVerifier<InterfaceAnalyzer.InterfaceAnalyzer>;

namespace InterfaceAnalyzer.Test;

public class Tests
{
    [Test]
    public async Task PublicConcreteClass_TriggersDiagnostic()
    {
        var testCode = @"
namespace TestNamespace;

public class {|#0:Foo|} 
{ 
}";

        var expected = VerifyCS.Diagnostic(InterfaceAnalyzer.DiagnosticId).WithLocation(0).WithArguments("Foo");
        await VerifyCS.VerifyAnalyzerAsync(testCode, expected);
    }

    [Test]
    public async Task InternalConcreteClass_NoDiagnostic()
    {
        var testCode = @"
namespace TestNamespace;

internal class Foo 
{ 
}";
        await VerifyCS.VerifyAnalyzerAsync(testCode); // No diagnostic expected
    }

    [Test]
    public async Task PublicAbstractClass_NoDiagnostic()
    {
        var testCode = @"
namespace TestNamespace;

public abstract class Foo 
{ 
}";
        await VerifyCS.VerifyAnalyzerAsync(testCode); // No diagnostic expected
    }
    
    [Test]
    public async Task InternalAbstractClass_NoDiagnostic()
    {
        var testCode = @"
namespace TestNamespace;

internal abstract class Foo 
{ 
}";
        await VerifyCS.VerifyAnalyzerAsync(testCode); // No diagnostic expected
    }

    [Test]
    public async Task PublicStaticClass_NoDiagnostic()
    {
        // Static classes are implicitly abstract and sealed
        var testCode = @"
namespace TestNamespace;

public static class Foo 
{ 
}";
        await VerifyCS.VerifyAnalyzerAsync(testCode); // No diagnostic expected
    }

    [Test]
    public async Task PublicSealedClass_TriggersDiagnostic()
    {
        var testCode = @"
namespace TestNamespace;

public sealed class {|#0:Foo|} 
{ 
}";
        var expected = VerifyCS.Diagnostic(InterfaceAnalyzer.DiagnosticId).WithLocation(0).WithArguments("Foo");
        await VerifyCS.VerifyAnalyzerAsync(testCode, expected);
    }

    [Test]
    public async Task DefaultInternalClass_NoDiagnostic()
    {
        // Classes without access modifiers default to internal
        var testCode = @"
namespace TestNamespace;

class Foo 
{ 
}";
        await VerifyCS.VerifyAnalyzerAsync(testCode); // No diagnostic expected
    }
}
