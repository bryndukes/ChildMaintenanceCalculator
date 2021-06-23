﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public class ObjectDumper
{
    private int _currentIndent;
    private readonly int _indentSize;
    private readonly StringBuilder _stringBuilder;
    private readonly Dictionary<object, int> _hashListOfFoundElements;
    private readonly char _indentChar;
    private readonly int _depth;
    private int _currentLine;

    private ObjectDumper(int depth, int indentSize, char indentChar)
    {
        _depth = depth;
        _indentSize = indentSize;
        _indentChar = indentChar;
        _stringBuilder = new StringBuilder();
        _hashListOfFoundElements = new Dictionary<object, int>();
    }

    public static string Dump(object element, int depth = 10, int indentSize = 2, char indentChar = ' ')
    {
        var instance = new ObjectDumper(depth, indentSize, indentChar);
        return instance.DumpElement(element, 1, true);
    }

    private string DumpElement(object element, int index = 1, bool isTopOfTree = false)
    {
        if (_currentIndent > _depth) { return null; }
        if (element == null || element is string)
        {
            Write(FormatValue(element));
        }
        else if (element is ValueType)
        {
            Type objectType = element.GetType();
            bool isWritten = false;
            if (objectType.IsGenericType)
            {
                Type baseType = objectType.GetGenericTypeDefinition();
                if (baseType == typeof(KeyValuePair<,>))
                {
                    isWritten = true;
                    Write("Key:");
                    _currentIndent++;
                    DumpElement(objectType.GetProperty("Key").GetValue(element, null));
                    _currentIndent--;
                    Write("Value:");
                    _currentIndent++;
                    DumpElement(objectType.GetProperty("Value").GetValue(element, null));
                    _currentIndent--;
                }
            }
            if (!isWritten)
            {
                Write(FormatValue(element));
            }
        }
        else
        {
            if (element is IEnumerable enumerableElement)
            {
                var position = 1;
                foreach (object item in enumerableElement)
                {
                    if (item is IEnumerable && !(item is string))
                    {
                        _currentIndent++;
                        DumpElement(item, 1);
                        _currentIndent--;
                    }
                    else
                    {
                        DumpElement(item, position);
                    }

                    position++;
                }
            }
            else
            {
                Type objectType = element.GetType();
                Write($"{objectType.Name} {index}:");
                if (!AlreadyDumped(element))
                {
                    _currentIndent++;
                    MemberInfo[] members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var memberInfo in members)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        var propertyInfo = memberInfo as PropertyInfo;

                        if (fieldInfo == null && (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length > 0))
                            continue;

                        var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                        object value;
                        try
                        {
                            value = fieldInfo != null
                                               ? fieldInfo.GetValue(element)
                                               : propertyInfo.GetValue(element, null);
                        }
                        catch (Exception e)
                        {
                            Write("{0} failed with:{1}", memberInfo.Name, (e.GetBaseException() ?? e).Message);
                            continue;
                        }

                        if (type.IsValueType || type == typeof(string))
                        {
                            Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                            Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }");

                            _currentIndent++;
                            DumpElement(value);
                            _currentIndent--;
                        }
                    }
                    _currentIndent--;
                }
            }
        }

        return isTopOfTree ? _stringBuilder.ToString() : null;
    }

    private bool AlreadyDumped(object value)
    {
        if (value == null)
            return false;
        if (_hashListOfFoundElements.TryGetValue(value, out int lineNo))
        {
            Write("(reference already dumped - line:{0})", lineNo);
            return true;
        }
        _hashListOfFoundElements.Add(value, _currentLine);
        return false;
    }

    private void Write(string value, params object[] args)
    {
        var space = new string(_indentChar, _currentIndent * _indentSize);

        if (args != null)
            value = string.Format(value, args);

        _stringBuilder.AppendLine(space + value);
        _currentLine++;
    }

    private string FormatValue(object o)
    {
        if (o == null)
            return ("null");

        if (o is DateTime)
            return (((DateTime)o).ToShortDateString());

        if (o is string)
            return "\"" + (string)o + "\"";

        if (o is char)
        {
            if (o.Equals('\0'))
            {
                return "''";
            }
            else
            {
                return "'" + (char)o + "'";
            }
        }

        if (o is ValueType)
            return (o.ToString());

        if (o is IEnumerable)
            return ("...");

        return ("{ }");
    }
}