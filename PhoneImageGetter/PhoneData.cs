namespace PhoneImageGetter
{
	public struct PhoneData
	{
		public long ItemId { get; }

		public string PhoneKey { get; }

		public PhoneData(long itemId, string phoneKey)
		{
			ItemId = itemId;
			PhoneKey = phoneKey;
		}
	}
}
