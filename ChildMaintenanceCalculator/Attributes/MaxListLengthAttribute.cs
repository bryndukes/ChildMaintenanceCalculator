using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Attributes
{
    public class MaxListLengthAttribute : ValidationAttribute
    {
        private readonly int _maxElements;
        public MaxListLengthAttribute(int maxElements)
        {
            _maxElements = maxElements;
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                return list.Count <= _maxElements;
            }
            return false;
        }
    }
}
