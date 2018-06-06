/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bind.Structures
{
    internal class EnumDefinition
    {
        private string _name, _type;

        public EnumDefinition()
        {
            CLSCompliant = true;
        }

        // Returns true if the enum contains a collection of flags, i.e. 1, 2, 4, 8, ...
        public bool IsFlagCollection
        {
            get; set;
        }

        public string Name
        {
            get => _name ?? "";
            set => _name = value;
        }

        // Typically 'long' or 'int'. Default is 'int'.
        public string Type
        {
            get => string.IsNullOrEmpty(_type) ? "int" : _type;
            set => _type = value;
        }

        private SortedDictionary<string, ConstantDefinition> _constantCollection = new SortedDictionary<string, ConstantDefinition>();

        public IDictionary<string, ConstantDefinition> ConstantCollection
        {
            get => _constantCollection;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _constantCollection.Clear();
                foreach (var item in value)
                {
                    _constantCollection.Add(item.Key, item.Value);
                }
            }
        }

        // Use only for debugging, not for code generation.
        public override string ToString()
        {
            return $"enum {Name} : {Type} {{ {ConstantCollection} }}";
        }

        public void Add(ConstantDefinition constantDefinition)
        {
            ConstantCollection.Add(constantDefinition.Name, constantDefinition);
        }

        public string Obsolete { get; set; }
        public bool IsObsolete => !string.IsNullOrEmpty(Obsolete);

        public bool CLSCompliant { get; set; }
    }

    internal class EnumCollection : IDictionary<string, EnumDefinition>
    {
        private SortedDictionary<string, EnumDefinition> _enumerations = new SortedDictionary<string, EnumDefinition>();

        // Return -1 for ext1, 1 for ext2 or 0 if no preference.
        private int OrderOfPreference(string ext1, string ext2)
        {
            // If one is empty and the other not, prefer the empty one (empty == core)
            // Otherwise check for Arb and Ext. To reuse the logic for the
            // empty check, let's try to remove first Arb, then Ext from the strings.
            int ret = PreferEmpty(ext1, ext2);
            if (ret != 0)
            {
                return ret;
            }

            ext1 = ext1.Replace("Arb", ""); ext2 = ext2.Replace("Arb", "");
            ret = PreferEmpty(ext1, ext2);
            if (ret != 0)
            {
                return ret;
            }

            ext1 = ext1.Replace("Ext", ""); ext2 = ext2.Replace("Ext", "");
            return PreferEmpty(ext1, ext2);
        }

        // Prefer the empty string over the non-empty.
        private int PreferEmpty(string ext1, string ext2)
        {
            if (string.IsNullOrEmpty(ext1) && !string.IsNullOrEmpty(ext2))
            {
                return -1;
            }

            if (string.IsNullOrEmpty(ext2) && !string.IsNullOrEmpty(ext1))
            {
                return 1;
            }

            return 0;
        }

        public void Add(EnumDefinition e)
        {
            Add(e.Name, e);
        }

        public void AddRange(EnumCollection enums)
        {
            foreach (EnumDefinition e in enums.Values)
            {
                Add(e);
            }
        }

        public void Add(string key, EnumDefinition value)
        {
            if (ContainsKey(key))
            {
                var e = this[key];
                foreach (var token in value.ConstantCollection.Values)
                {
                    Utilities.Merge(e, token);
                }
            }
            else
            {
                _enumerations.Add(key, value);
            }
        }

        public bool ContainsKey(string key)
        {
            return _enumerations.ContainsKey(key);
        }

        public ICollection<string> Keys => _enumerations.Keys;

        public bool Remove(string key)
        {
            return _enumerations.Remove(key);
        }

        public bool TryGetValue(string key, out EnumDefinition value)
        {
            return _enumerations.TryGetValue(key, out value);
        }

        public ICollection<EnumDefinition> Values => _enumerations.Values;

        public EnumDefinition this[string key]
        {
            get => _enumerations[key];
            set => _enumerations[key] = value;
        }

        public void Add(KeyValuePair<string, EnumDefinition> item)
        {
            _enumerations.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _enumerations.Clear();
        }

        public bool Contains(KeyValuePair<string, EnumDefinition> item)
        {
            return _enumerations.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, EnumDefinition>[] array, int arrayIndex)
        {
            _enumerations.CopyTo(array, arrayIndex);
        }

        public int Count => _enumerations.Count;

        public bool IsReadOnly => (_enumerations as IDictionary<string, EnumDefinition>).IsReadOnly;

        public bool Remove(KeyValuePair<string, EnumDefinition> item)
        {
            return _enumerations.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, EnumDefinition>> GetEnumerator()
        {
            return _enumerations.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _enumerations.GetEnumerator();
        }
    }
}