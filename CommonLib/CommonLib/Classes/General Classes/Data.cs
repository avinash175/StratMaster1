using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommonLib
{
    public class DictionaryBindingList<TKey, TValue> : BindingList<Pair<TKey, TValue>> 
    { 
        private readonly IDictionary<TKey, TValue> data; 
        public DictionaryBindingList(IDictionary<TKey, TValue> data) 
        { 
            this.data = data;
            Reset();
        }
        public void Reset() 
        { 
            bool oldRaise = RaiseListChangedEvents; 
            RaiseListChangedEvents = false;
            try
            { 
                Clear();
                foreach (TKey key in data.Keys) 
                { 
                    Add(new Pair<TKey, TValue>(key, data)); 
                }
            }
            finally 
            { 
                RaiseListChangedEvents = oldRaise; 
                ResetBindings(); 
            }
        }
    }

    public sealed class Pair<TKey, TValue> 
    { 
        private readonly TKey key;
        private readonly IDictionary<TKey, TValue> data;
        public Pair(TKey key, IDictionary<TKey, TValue> data)
        {
            this.key = key;
            this.data = data;
        }
        public TKey Key 
        { 
            get 
            { 
                return key; 
            } 
        } 
        public TValue Value 
        { 
            get 
            { 
                TValue value;
                data.TryGetValue(key, out value);
                return value;
            }
            set 
            { 
                data[key] = value; 
            } 
        } 
    }
}
