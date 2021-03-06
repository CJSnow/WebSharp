﻿//
// ScriptObjectHelpers.cs
//

using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebSharpJs.Script
{

    public static class ScriptObjectHelper
    {

        public static IDictionary<string, object> ScriptableTypeToDictionary(object value)
        {
            var parmType = value.GetType();
            var propertyMappings = new Dictionary<string, object>();

            if (parmType.IsAttributeDefined<ScriptableTypeAttribute>(false))
            {
                
                var scriptAlias = string.Empty;
                var properties = parmType.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < properties.Length; i++)
                {
                    var propertyInfo = properties[i];

                    if (propertyInfo.GetIndexParameters().Length == 0 && propertyInfo.GetMethod != null)
                    {
                        scriptAlias = propertyInfo.Name;
                        if (propertyInfo.IsDefined(typeof(ScriptableMemberAttribute), false))
                        {
                            var att = propertyInfo.GetCustomAttribute<ScriptableMemberAttribute>(false);
                            scriptAlias = (att.ScriptAlias ?? scriptAlias);
                        }

                        propertyMappings.Add(scriptAlias, propertyInfo.GetValue(value));
                    }
                }
            }

            return propertyMappings;
        }

        public enum ScriptMemberMappingDirection
        {
            ScriptAliasToMember,
            MemberToScriptAlias
        }

        public static IDictionary<string, string> GetScriptMemberMappings (Type type, ScriptMemberMappingDirection direction = ScriptMemberMappingDirection.MemberToScriptAlias )
        {
            var map = new Dictionary<string, string>();
            var scriptAlias = string.Empty;

            // add properties
            foreach (PropertyInfo pi in type.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (pi.IsSpecialName)
                    continue;

                scriptAlias = pi.Name;
                if (pi.IsDefined(typeof(ScriptableMemberAttribute), false))
                {
                    var att = pi.GetCustomAttribute<ScriptableMemberAttribute>(false);
                    scriptAlias = (att.ScriptAlias ?? scriptAlias);
                }

                if (direction == ScriptMemberMappingDirection.MemberToScriptAlias)
                    map[pi.Name] = scriptAlias;
                else
                    map[scriptAlias] = pi.Name;
            }

            // add events
            foreach (EventInfo ei in type.GetTypeInfo().GetEvents())
            {
                if (ei.IsSpecialName)
                    continue;

                scriptAlias = ei.Name;
                if (ei.IsDefined(typeof(ScriptableMemberAttribute), false))
                {
                    var att = ei.GetCustomAttribute<ScriptableMemberAttribute>(false);
                    scriptAlias = (att.ScriptAlias ?? scriptAlias);
                }

                if (direction == ScriptMemberMappingDirection.MemberToScriptAlias)
                    map[ei.Name] = scriptAlias;
                else
                    map[scriptAlias] = ei.Name;
            }

            // add methods
            foreach (MethodInfo mi in type.GetTypeInfo().GetMethods())
            {
                if (mi.IsSpecialName)
                    continue;

                scriptAlias = mi.Name;
                if (mi.IsDefined(typeof(ScriptableMemberAttribute), false))
                {
                    var att = mi.GetCustomAttribute<ScriptableMemberAttribute>(false);
                    scriptAlias = (att.ScriptAlias ?? scriptAlias);
                }

                if (direction == ScriptMemberMappingDirection.MemberToScriptAlias)
                    map[mi.Name] = scriptAlias;
                else
                    map[scriptAlias] = mi.Name;
            }
            return map;
        }

        public static ScriptObjectProxy AnonymousObjectToScriptObjectProxy(dynamic source)
        {
            IDictionary<string, object> dict = source;
            
            // The key `websharp_id` represents a wrapped proxy object
            if (dict.ContainsKey("websharp_id"))
            {
                return new ScriptObjectProxy(source);
            }
            return null;
        }

        public static bool DictionaryToScriptableType(IDictionary<string, object> parm, object obj)
        {

            var parmType = obj.GetType();

            if (parmType.IsAttributeDefined<ScriptableTypeAttribute>(false))
            {
                var mappings = GetScriptMemberMappings(parmType, ScriptMemberMappingDirection.ScriptAliasToMember);

                foreach(var key in parm.Keys)
                {
                    if (mappings.ContainsKey(key))
                    {
                        var pi = parmType.GetTypeInfo().GetProperty(mappings[key]);
                        if (pi.SetMethod != null)
                        {
                            pi.SetValue(obj, parm[key]);
                        }
                    }
                }

            }

            return true;
        }
    }
}