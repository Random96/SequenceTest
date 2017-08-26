namespace EmlSoft.Sequence
{
	/// <summary>
	/// Sequence interface
	/// </summary>
	public interface ISequence
	{
		/// <summary>
		/// Curent value
		/// If Sequence create - contain last issued in library value
		/// after call NextValue they contain owned, thread independent value
		/// </summary>
		/// <returns></returns>
		string CurrentValue();

		/// <summary>
		/// calculate new value
		/// </summary>
		/// <returns></returns>
		string NextValue();
	}
}