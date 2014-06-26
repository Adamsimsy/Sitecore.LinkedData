using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace LinkedData
{
    public static class DependencyResolver
    {
        private static readonly object Padlock = new object();
        private static IWindsorContainer _instance;

        public static IWindsorContainer Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new WindsorContainer());
                }
            }
            set
            {
                lock (Padlock)
                {
                    _instance = value;
                }
            }
        }
    }
}
