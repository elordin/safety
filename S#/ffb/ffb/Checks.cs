using NUnit.Framework;
using SafetySharp.Analysis;
using SafetySharp.Modeling;
using ffb.Modelling;

class Checks
{
    [Test]
    public void DCCA()
    {
        var model = new Model();
        var result = SafetyAnalysis.AnalyzeHazard(model, model.Hazard);

        System.Console.Write(result);

        Assert.IsTrue(result.IsComplete);
    }
}
