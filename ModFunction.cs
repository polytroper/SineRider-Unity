using System;
using UnityEngine;
using System.Collections.Generic;

namespace YAMP
{
    class ModFunction : StandardFunction
    {
        public override Value Perform(Value argument)
        {
            if (argument is ScalarValue)
                return argument;
            else if (argument is MatrixValue)
            {
                var m = argument as MatrixValue;

				if(m.DimensionX == 1)
					return GetVectorMod(m.GetColumnVector(1));
				else if(m.DimensionY == 1)
					return GetVectorMod(m.GetRowVector(1));
				else
				{
					var M = new MatrixValue(1, m.DimensionX);
					
					for(var i = 1; i <= m.DimensionX; i++)
						M[1, i] = GetVectorMod(m.GetColumnVector(i));
					
					return M;
				}
            }

            throw new OperationNotSupportedException("mod", argument);
        }
		
        ScalarValue GetVectorMod(MatrixValue vec)
        {
            //Debug.Log(vec.Length+" | "+vec[1].Value);
            return new ScalarValue(vec[1].Value%vec[2].Value);

            //var buf = new ScalarValue();
            //var max = double.NegativeInfinity;
            //var temp = 0.0;

            //for(var i = 1; i <= vec.Length; i++)
            //{
            //    temp = vec[i].Abs().Value;

            //    if (temp > max)
            //    {
            //        buf = vec[i];
            //        max = temp;
            //    }
            //}

            //return buf;
        }
    }
}
