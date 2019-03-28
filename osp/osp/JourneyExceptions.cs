using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osp
{

    /*
     * Список всех возможных специфических исключений и их обработчиков, которые могут возникнуть
     * при работе программы, и кодов ошибок для них и других исключений.
     */

    public class ErrorCodes
    {
        // Код успеха, означет, что ошибки не произошло.
        public const string Success = "0x000000000";
        // Неизвестная ошибка.
        public const string UnknownError = "0x000000001";
        // Ошибка ввода-вывода.
        public const string IOError = "0x000000002";
        
        // Неизвестное ключевое слово в файле TSPLib.
        public const string UnknownKeywordProblemError = "0x000000003";
        // Неверный формат значения в файле TSPLib.
        public const string InvalidTSPLibValueError = "0x000000004";
        // Вызов нереализованного функционала.
        public const string NotImplementedTSPError = "0x000000005";
   
    }

    // Не был указан обязательный параметр.
    [Serializable]
    public class ProblemFileIsNotSpecifiedException : Exception
    {
        public ProblemFileIsNotSpecifiedException() { }
        public ProblemFileIsNotSpecifiedException(string message) : base(message) { }
        public ProblemFileIsNotSpecifiedException(string message, Exception inner) : base(message, inner) { }
        protected ProblemFileIsNotSpecifiedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    
    // Неверный формат строки параметров.
    [Serializable]
    public class InvalidParameterLineException : Exception
    {
        private int lineNumber = -1;
        public int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }
            
        public InvalidParameterLineException() { }
        public InvalidParameterLineException(string message) : base(message) { }
        public InvalidParameterLineException(string message, int lineNum) : base(message) 
        {
            LineNumber = lineNum;
        }
        public InvalidParameterLineException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParameterLineException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    // Неверный формат ключа параметра.
    [Serializable]
    public class InvalidParameterKeyException : Exception
    {
        public InvalidParameterKeyException() { }
        public InvalidParameterKeyException(string message) : base(message) { }
        public InvalidParameterKeyException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParameterKeyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    // Неверный формат значения параметра.
    [Serializable]
    public class InvalidParameterValueException : Exception
    {
        public InvalidParameterValueException() { }
        public InvalidParameterValueException(string message) : base(message) { }
        public InvalidParameterValueException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParameterValueException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    // Неверный атрибут XML.
    [Serializable]
    public class InvalidXMLAttribException : Exception
    {
        public InvalidXMLAttribException() { }
        public InvalidXMLAttribException(string message) : base(message) { }
        public InvalidXMLAttribException(string message, Exception inner) : base(message, inner) { }
        protected InvalidXMLAttribException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    // Неизвестное ключевое слово в файле TSPLib.
    [Serializable]
    public class UnknownProblemKeywordException : Exception
    {
        public UnknownProblemKeywordException() { }
        public UnknownProblemKeywordException(string message) : base(message) { }
        public UnknownProblemKeywordException(string message, Exception inner) : base(message, inner) { }
        protected UnknownProblemKeywordException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    // Неверное значение параметра файла TSPLib.
    [Serializable]
    public class InvalidTSPLibValueException : Exception
    {
        public InvalidTSPLibValueException() { }
        public InvalidTSPLibValueException(string message) : base(message) { }
        public InvalidTSPLibValueException(string message, Exception inner) : base(message, inner) { }
        protected InvalidTSPLibValueException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    // Ошибке при обращении к нереализованной спецификации TSPLib.
    public class NotImplementedTSPException : Exception
    {
        public NotImplementedTSPException() { }
        public NotImplementedTSPException(string message) : base(message) { }
        public NotImplementedTSPException(string message, Exception inner) : base(message, inner) { }
        protected NotImplementedTSPException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
 
}
