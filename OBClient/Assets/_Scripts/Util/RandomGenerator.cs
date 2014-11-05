using System.Collections;

public class RandomGenerator
{
	private int m_Value = 0;

	public RandomGenerator() 
	{ 
	}

	public RandomGenerator( int aSeed )
	{
		m_Value = aSeed;
	}
	
	// 
	// Summary:
	//     Returns a nonnegative random number.
	//
	// Returns:
	//     A 32-bit signed integer greater than or equal to zero and less than System.Int32.MaxValue.
	public int Next()
	{
		m_Value = m_Value * 0x08088405 + 1;
		return (int)( m_Value & 0x7FFFFFFF );
	}
	
	//
	// Summary:
	//     Returns a nonnegative random number less than the specified maximum.
	//
	// Parameters:
	//   maxValue:
	//     The exclusive upper bound of the random number to be generated. maxValue
	//     must be greater than or equal to zero.
	//
	// Returns:
	//     A 32-bit signed integer greater than or equal to zero, and less than maxValue;
	//     that is, the range of return values ordinarily includes zero but not maxValue.
	//     However, if maxValue equals zero, maxValue is returned.
	//
	// Exceptions:
	//   System.ArgumentOutOfRangeException:
	//     maxValue is less than zero.
	public int Next( int maxValue )
	{
		if ( maxValue < 0 )
			throw new System.ArgumentOutOfRangeException( "maxValue must be greater than or equal to zero." );
		else if ( maxValue > 0 )
			return Next() % ( maxValue );
		
		return Next() * 0;
	}
	
	//
	// Summary:
	//     Returns a random number within a specified range.
	//
	// Parameters:
	//   minValue:
	//     The inclusive lower bound of the random number returned.
	//
	//   maxValue:
	//     The exclusive upper bound of the random number returned. maxValue must be
	//     greater than or equal to minValue.
	//
	// Returns:
	//     A 32-bit signed integer greater than or equal to minValue and less than maxValue;
	//     that is, the range of return values includes minValue but not maxValue. If
	//     minValue equals maxValue, minValue is returned.
	//
	// Exceptions:
	//   System.ArgumentOutOfRangeException:
	//     minValue is greater than maxValue.
	public int Next( int minValue, int maxValue )
	{
		if ( minValue > maxValue )
			throw new System.ArgumentOutOfRangeException( "maxValue must be greater than or equal to minValue." );
		else if ( minValue < maxValue )
			return minValue + Next() % ( maxValue - minValue );
		
		return Next() * 0 + maxValue;
	}
	
	//
	// Summary:
	//     Fills the elements of a specified array of bytes with random numbers.
	//
	// Parameters:
	//   buffer:
	//     An array of bytes to contain random numbers.
	//
	// Exceptions:
	//   System.ArgumentNullException:
	//     buffer is null.
	public void NextBytes( byte[] buffer )
	{
		if ( buffer == null )
			throw new System.ArgumentOutOfRangeException( "buffer should not be null." );
		
		for ( int i = 0; i < buffer.Length; ++i )
			buffer[i] = (byte)Next();
	}
	
	// HOLD!
	//
	// Summary:
	//     Returns a random number between 0.0 and 1.0.
	//
	// Returns:
	//     A double-precision floating point number greater than or equal to 0.0, and
	//     less than 1.0.
	public double NextDouble()
	{
		return 0.0;
	}
}
