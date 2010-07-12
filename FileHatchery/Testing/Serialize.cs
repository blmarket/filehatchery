using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Testing.SerializationTest
{
    public class Test
    {
        public static void SerializationTest()
        {
            Test t = new Test();
            t.SerializeCollection("coll.xml");
        }

        private void SerializeCollection(string filename)
        {
            Employees<Employee> Emps = new Employees<Employee>();
            // Note that only the collection is serialized -- not the 
            // CollectionName or any other public property of the class.
            Emps.CollectionName = "Employees";
            Emps.Add(new Employee("JOhn", "100xxx"));
            Employee John100 = new Employee("John", "100xxx");
            Emps.Add(John100);

            Dictionary<string, string> dict = new Dictionary<string,string>();
            dict.Add("asdf", "news");
            dict.Add("kkk", "hhh");
            DictToArray darray = new DictToArray(dict);
            XmlSerializer x = new XmlSerializer(typeof(DictToArray));
            TextWriter writer = new StreamWriter(filename);
            x.Serialize(writer, darray);
        }
    }

    public class DictToArray : ICollection
    {
        private ArrayList m_arr = new ArrayList();

        public DictToArray(Dictionary<string,string> dict)
        {
            foreach (var item in dict)
            {
                m_arr.Add(new Employee(item.Key, item.Value));
            }
        }

        public Employee this[int index]
        {
            get { return (Employee)m_arr[index]; }
        }

        public void CopyTo(Array array, int index)
        {
            m_arr.CopyTo(array, index);
        }

        public int Count
        {
            get { return m_arr.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public IEnumerator GetEnumerator()
        {
            return m_arr.GetEnumerator();
        }

        public void Add(Employee item)
        {
            m_arr.Add(item);
        }
    }

    public class Employees<T> : ICollection
    {
        public string CollectionName;
        private ArrayList empArray = new ArrayList();

        public T this[int index]
        {
            get { return (T)empArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            empArray.CopyTo(a, index);
        }
        public int Count
        {
            get { return empArray.Count; }
        }
        public object SyncRoot
        {
            get { return this; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public IEnumerator GetEnumerator()
        {
            return empArray.GetEnumerator();
        }

        public void Add(T newEmployee)
        {
            empArray.Add(newEmployee);
        }
    }

    public class Employee
    {
        public string EmpName;
        public string EmpID;
        public Employee() { }
        public Employee(string empName, string empID)
        {
            EmpName = empName;
            EmpID = empID;
        }
    }
}