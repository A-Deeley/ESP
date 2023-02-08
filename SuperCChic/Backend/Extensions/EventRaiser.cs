using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Extensions
{
    public static class EventRaiser
    {
        public static void Raise(this EventHandler handler, object sender) => handler?.Invoke(sender, EventArgs.Empty);
        public static void Raise(this EventHandler<SGIEventArgs> handler, object sender, string value) => handler?.Invoke(sender, new(value));
        public static void Raise(this EventHandler<SGIEventArgs> handler, object sender, string value, Product model) => handler?.Invoke(sender, new(value, model));
    }
}
