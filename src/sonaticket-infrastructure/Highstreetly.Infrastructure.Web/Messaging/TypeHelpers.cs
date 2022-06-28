using System;

namespace Highstreetly.Infrastructure.Messaging
{
    public static class TypeHelpers
    {
        public static object ToType<T>(this object obj)
        {
            var typeName = typeof(T).Name.Substring(1);
            var namesppace = typeof(T).Namespace;
            
            //create instance of T type object:
            var instanceToCreate = Type.GetType($"{namesppace}.{typeName}");
            var tmp = Activator.CreateInstance(instanceToCreate); 

            //loop through the properties of the object you want to covert:          
            foreach (var pi in obj.GetType().GetProperties())
            {
                try 
                {   
                    //get the value of property and try 
                    //to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null), null);
                }
                catch { }
            }  

            //return the T type object:         
            return tmp; 
        }
    }
}