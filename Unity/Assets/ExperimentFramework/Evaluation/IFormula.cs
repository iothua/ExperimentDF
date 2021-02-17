namespace ExperimentFramework.Evaluation
{
    /// <summary>
    /// 公式运算，用于检测某些数值是否合理
    /// </summary>
    public interface IFormula
    {
        FormulaMessage Formula(string[] args,params string[] anwser);
    }
}