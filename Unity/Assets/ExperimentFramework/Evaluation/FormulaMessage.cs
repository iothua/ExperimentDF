using System.Collections.Generic;

namespace ExperimentFramework.Evaluation
{
    public struct FormulaMessage
    {
        public bool IsSuccess;

        /// <summary>
        /// 错误消息
        /// </summary>
        public List<string> ErrorMessages;

        public void AddError(string msg)
        {
            if(ErrorMessages==null)
                ErrorMessages=new List<string>();

            ErrorMessages.Add(msg);
        }

        public string ErrorStr
        {
            get
            {
                string error = string.Empty;

                if (ErrorMessages == null)
                    return error;
                
                foreach (var message in ErrorMessages)
                {
                    error += message + "\r\n";
                }

                return error;
            }
        }
    }
}