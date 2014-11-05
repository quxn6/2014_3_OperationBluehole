using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    public class RandomGenerator
    {
        private int m_Value = 0;
        public RandomGenerator() { }
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

    class MinHeap<T> : IEnumerable<T> where T : IComparable<T>
    {
        private int size;
        private List<T> elements;

        public MinHeap()
        {
            elements = new List<T>();
            size = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for ( int i = 0; i < size; ++i )
                yield return elements[i];
        }

        IEnumerator IEnumerable.GetEnumerator() 
        {
            return GetEnumerator();
        }

        private void Heapify( int idx )
        {
            if ( 2 * idx > size ) //children이 없으면 종료
                return;

            int largeIdx = 2 * idx;
            if ( 2 * idx + 1 <= size && elements[largeIdx].CompareTo( elements[2 * idx + 1] ) > 0 )
                largeIdx = 2 * idx + 1;

            T tempElement;

            if ( elements[idx].CompareTo( elements[largeIdx] ) > 0 ) 
            {
                tempElement = elements[largeIdx];
                elements[largeIdx] = elements[idx];
                elements[idx] = tempElement;

                Heapify( largeIdx ); //교체 후에는 교체한 자리에서 다시 heapify 실행
            }

            return;
        }

        public void Push( T newElement )
        {
            int currentIdx = size;

            if ( elements.Count > size )
                elements[size] = newElement;
            else
                elements.Add( newElement );
            
            ++size;
            T tempElement;

            while ( currentIdx > 0 && elements[currentIdx].CompareTo( elements[currentIdx / 2] ) < 0 )
            {
                tempElement = elements[currentIdx];
                elements[currentIdx] = elements[currentIdx / 2];
                elements[currentIdx / 2] = tempElement;

                currentIdx = currentIdx / 2;
            }
        }

        public void Pop()
        {
            if ( size == 0 )
                return;

            elements[0] = elements[--size];
            Heapify( 0 );
        }

        // 길찾기 할 때 키 값 감소만 생기니까 일단 이것만 만들자
        public bool DecreaseKeyValue( T targetElement )
        {
            // 조심해!
            // 해당 element를 찾아야 한다
            // 선형으로 다시 찾는 게 최선인가요?!
            int currentIdx = -1;
            for ( int i = 0; i < size; ++i )
            {
                if ( targetElement.Equals(elements[i]))
                {
                    currentIdx = i;
                    break;
                }
            }

            if ( currentIdx == -1 )
                return false;

            T tempElement;

            while ( currentIdx > 0 && elements[currentIdx].CompareTo( elements[currentIdx / 2] ) < 0 )
            {
                tempElement = elements[currentIdx];
                elements[currentIdx] = elements[currentIdx / 2];
                elements[currentIdx / 2] = tempElement;

                currentIdx = currentIdx / 2;
            }

            return true;
        }

        public T Peek() 
        {
            if ( size == 0 )
                return default(T);

            return elements[0]; 
        }

        public int Count { get { return size; } }
    }
}

