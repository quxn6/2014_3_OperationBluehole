using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    class MinHeap<T>
    {
        private int size;
        private List<T> elements;
        private Comparer<T> comparer;

        public MinHeap( Comparer<T> comparer )
        {
            elements = new List<T>();
            this.comparer = comparer;
            size = 0;
        }

        private void Heapify( int idx )
        {
            if ( 2 * idx > size ) //children이 없으면 종료
                return;

            int largeIdx = 2 * idx;
            if ( 2 * idx + 1 <= size && comparer.Compare( elements[largeIdx], elements[2 * idx + 1] ) > 0 )
                largeIdx = 2 * idx + 1;

            T tempElement;

            if ( comparer.Compare( elements[idx], elements[largeIdx] ) > 0 ) 
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

            while ( currentIdx > 0 && comparer.Compare( elements[currentIdx], elements[currentIdx / 2] ) < 0 )
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

            while ( currentIdx > 0 && comparer.Compare( elements[currentIdx], elements[currentIdx / 2] ) < 0 )
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

