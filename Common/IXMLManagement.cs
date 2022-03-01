using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IXMLManagement
    {
        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        void Create(string filename, byte[] sign);


        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        [FaultContract(typeof(MyException))]
        void Delete(string filename, byte[] sign);

        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        string Read(string filename, byte[] sign);

        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        void AddProfessor(string filename, string poruka, byte[] sign, byte[] sign2);

        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        void AddStudent(string filename, string poruka, byte[] sign, byte[] sign2);

        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        void AddSubject(string filename, string poruka, byte[] sign, byte[] sign2);

        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        void DeleteProfessor(string filename, string poruka, byte[] sign, byte[] sign2);

        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        void DeleteStudent(string filename, string poruka, byte[] sign, byte[] sign2);

        [OperationContract]
        [FaultContract(typeof(MyException))]
        [FaultContract(typeof(SecurityException))]
        void DeleteSubject(string filename, string poruka, byte[] sign, byte[] sign2);


    }
}
