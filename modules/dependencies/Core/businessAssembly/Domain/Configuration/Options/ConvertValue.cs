using System;

namespace Transversals.Business.Core.Domain.Configuration.Options
{
    public static class ConvertValue
    {
        public static T GetValue<T>(this Option opt)
        {
            T val;
            TypeCode tc = Type.GetTypeCode(typeof(T));
            val = (T)Convert.ChangeType(opt.Value, tc);
            return val;
        }
    }
}
