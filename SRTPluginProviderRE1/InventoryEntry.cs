using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1
{
	[DebuggerDisplay("{_DebuggerDisplay,nq}")]
	[StructLayout(LayoutKind.Sequential)]
	public struct InventoryEntry : IEquatable<InventoryEntry>, IEqualityComparer<InventoryEntry>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string _DebuggerDisplay
		{
			get => string.Format("[#{0}] Item {1} ({2}) Quantity {3} IsEquipped {4}", SlotPosition, ItemName, (int)Item, Quantity, IsEquipped);
		}

        internal int[] _data;

        public ItemEnumeration Item => (ItemEnumeration)_data[0];
        public string ItemName => Item.ToString();
        public int Quantity => _data[1];
		public int SlotPosition { get; internal set; }
		public bool IsEquipped { get; internal set; }

        public bool Equals(InventoryEntry other) => this == other;
        public bool Equals(InventoryEntry x, InventoryEntry y) => x == y;
        public override bool Equals(object obj)
        {
            if (obj is InventoryEntry)
                return this == (InventoryEntry)obj;
            else
                return base.Equals(obj);
        }
        public static bool operator ==(InventoryEntry obj1, InventoryEntry obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;

            if (ReferenceEquals(obj1, null) || ReferenceEquals(obj1._data, null))
                return false;

            if (ReferenceEquals(obj2, null) || ReferenceEquals(obj2._data, null))
                return false;

            return obj1.SlotPosition == obj2.SlotPosition && obj1._data.SequenceEqual(obj2._data);
        }
        public static bool operator !=(InventoryEntry obj1, InventoryEntry obj2) => !(obj1 == obj2);

        public override int GetHashCode() => SlotPosition ^ _data.Aggregate((int p, int c) => p ^ c);
        public int GetHashCode(InventoryEntry obj) => obj.GetHashCode();

        public override string ToString() => _DebuggerDisplay;

        public InventoryEntry Clone()
        {
            InventoryEntry clone = new InventoryEntry() { _data = new int[this._data.Length] };
            for (int i = 0; i < this._data.Length; ++i)
                clone._data[i] = this._data[i];
            clone.SlotPosition = this.SlotPosition;
            clone.IsEquipped = this.IsEquipped;
            return clone;
        }

        public static InventoryEntry Clone(InventoryEntry subject) => subject.Clone();
    }
}
