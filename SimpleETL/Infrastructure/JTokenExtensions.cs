using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Imato.SimpleETL
{
    public static class JTokenExtensions
    {
        public static object TryGetObject(this JToken jtoken, Type type)
        {
            try
            {
                return jtoken.ToObject(type);
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<IEtlRow> GetRows(this JToken jt, Type type, string jsonPath, IEtlDataFlow flow)
        {
            if (jt != null && jt.HasValues)
            {
                if (!string.IsNullOrEmpty(jsonPath))
                {
                    foreach (var p in jsonPath.Split('.'))
                    {
                        if (jt[p] != null && jt[p].HasValues)
                            jt = jt[p];
                    }
                }

                if (jt != null && jt.HasValues)
                {
                    foreach (var r in jt)
                    {
                        var dto = r.TryGetObject(type);
                        if (dto != null)
                        {
                            yield return ObjectMapper.GetEtlRow(dto, flow);
                        }
                    }
                }
            }
        }
    }
}