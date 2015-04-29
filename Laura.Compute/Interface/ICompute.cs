using System;
using System.Collections;

namespace Laura.Compute
{
    public interface ICompute
    {
        ExpressSlice ExpressSlice { get; set; }
        ExpressSlice[] Arguments { get; set; } //get; 
        object Compute(ExpressSchema expressSchema, object objOrHash);
    }

    public abstract class ComputeBase : ICompute
    {
        public ExpressSlice ExpressSlice { get; set; }
        public ExpressSlice[] Arguments { get; set; }

        public abstract object Compute(ExpressSchema expressSchema, object objOrHash);

        protected object ArgumentsObject(int index, ExpressSchema expressSchema, object objOrHash)
        {
            if (Arguments.Length == 1 && Arguments[0].ExpressType == ExpressType.ArrayList)
            {
                ExpressSlice argument = Arguments[0];
                if (argument != null)
                {
                    ComputeBase computer = argument.Computer as ComputeBase;
                    if (computer != null)
                    {
                        return computer.ArgumentsObject(index, expressSchema, objOrHash);
                    }
                }
            }

            return Arguments[index].Compute(expressSchema, objOrHash);
        }
        protected string ArgumentsString(int index, ExpressSchema expressSchema, object objOrHash)
        {
            if (Arguments.Length == 1 && Arguments[0].ExpressType == ExpressType.ArrayList)
            {
                ExpressSlice argument = Arguments[0];
                if (argument != null)
                {
                    ComputeBase computer = argument.Computer as ComputeBase;
                    if (computer != null)
                    {
                        return computer.ArgumentsString(index, expressSchema, objOrHash);
                    }
                }
            }

            return Arguments[index].ComputeString(expressSchema, objOrHash);
        }
        protected double ArgumentsDouble(int index, ExpressSchema expressSchema, object objOrHash)
        {
            if (Arguments.Length == 1 && Arguments[0].ExpressType == ExpressType.ArrayList)
            {
                ExpressSlice argument = Arguments[0];
                if (argument != null)
                {
                    ComputeBase computer = argument.Computer as ComputeBase;
                    if (computer != null)
                    {
                        return computer.ArgumentsDouble(index, expressSchema, objOrHash);
                    }
                }
            }

            return Arguments[index].ComputeDouble(expressSchema, objOrHash);
        }
        protected bool ArgumentsBoolean(int index, ExpressSchema expressSchema, object objOrHash)
        {
            if (Arguments.Length == 1 && Arguments[0].ExpressType == ExpressType.ArrayList)
            {
                ExpressSlice argument = Arguments[0];
                if (argument != null)
                {
                    ComputeBase computer = argument.Computer as ComputeBase;
                    if (computer != null)
                    {
                        return computer.ArgumentsBoolean(index, expressSchema, objOrHash);
                    }
                }
            }

            return Arguments[index].ComputeDoolean(expressSchema, objOrHash);
        }
        protected ArrayList ArgumentsArray(int index, ExpressSchema expressSchema, object objOrHash)
        {
            if (Arguments.Length == 1 && Arguments[0].ExpressType == ExpressType.ArrayList)
            {
                ExpressSlice argument = Arguments[0];
                if (argument != null)
                {
                    ComputeBase computer = argument.Computer as ComputeBase;
                    if (computer != null)
                    {
                        ArrayList result = computer.ArgumentsArray(index, expressSchema, objOrHash);
                        return result ?? argument.ComputeArray(expressSchema, objOrHash);
                    }
                }
            }

            return Arguments[index].ComputeArray(expressSchema, objOrHash);
        }
        protected DateTime ArgumentsDate(int index, ExpressSchema expressSchema, object objOrHash)
        {
            if (Arguments.Length == 1 && Arguments[0].ExpressType == ExpressType.ArrayList)
            {
                ExpressSlice argument = Arguments[0];
                if (argument != null)
                {
                    ComputeBase computer = argument.Computer as ComputeBase;
                    if (computer != null)
                    {
                        return computer.ArgumentsDate(index, expressSchema, objOrHash);
                    }
                }
            }

            return Arguments[index].ComputeDate(expressSchema, objOrHash);
        }
        
        protected ExpressType ArgumentsType(int index)
        {
            return Arguments[index].ExpressType;
        }
    }
}
