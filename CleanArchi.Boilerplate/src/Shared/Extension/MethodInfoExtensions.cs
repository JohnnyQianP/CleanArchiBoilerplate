﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.Shared.Extension;

public static class MethodInfoExtensions
{
    public static string GetFullName(this MethodInfo method)
    {
        if (method.DeclaringType == null)
        {
            return $@"{method.Name}";
        }

        return $"{method.DeclaringType.FullName}.{method.Name}";
    }
}
