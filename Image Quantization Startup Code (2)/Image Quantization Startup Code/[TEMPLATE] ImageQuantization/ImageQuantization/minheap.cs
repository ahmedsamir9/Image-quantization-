using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
     class MinHeap
    {
        int capacity;
        int currentSize;
       public  vertix_data [] Heap;
       public  int[] indexes; //will be used to decrease the key


        public MinHeap(int capacity)                                          /*      O(1)      */ 
        {
            this.capacity = capacity;                                               //O(1)
            Heap = new  vertix_data [capacity + 1];                                 //O(1)
            indexes = new int[capacity];                                            //O(1)
            Heap[0] = new vertix_data();                                            //O(1) 
            Heap[0].weight= int.MinValue;                                           //O(1)
            Heap[0].pos = -1;                                                       //O(1)
            currentSize = 0;                                                        //O(1)
        }




         /*                                     */
        public void decrease(vertix_data we, int pos)                       /*    O(log V)    */    // Where V is number of levels
        {
            //get the index which key's needs a decrease;
            int index = indexes[pos];                                           //O(1)
            //get the node and update its value
            Heap[index] = we;                                                   //O(1)
           
            HeapUp(index);                                                      //O(1)
        }



      
         // Add new element in the  heap 
        public void insert(vertix_data x)                              /*    O(log V)    */   // Where V is number of levels
        {
            currentSize++;                                                  //O(1)
            Heap[currentSize] = x;                                          //O(1)
            indexes[x.pos] = currentSize;                                   //O(1)
            HeapUp(currentSize);                                            //O(log N)
        }




         // update postions of the elements of the Heap 
        public void HeapUp(int pos)                                                            /*    O(log N)    */   // Where N is number of levels
        {
            int parentIdx = pos / 2;                                                               //O(1) 
            int currentIdx = pos;                                                                  //O(1)
            while (currentIdx > 0 && Heap[parentIdx].weight > Heap[currentIdx].weight)             //O(log N)
            {                                                                                   
                vertix_data currentNode = Heap[currentIdx];                                        //O(1)
                vertix_data parentNode = Heap[parentIdx];                                          //O(1)
                 
                //swap the positions                                                        
                indexes[currentNode.pos] = parentIdx;                                              //O(1)
                indexes[parentNode.pos] = currentIdx;                                              //O(1)
                swap(currentIdx, parentIdx);                                                       //O(1)
                currentIdx = parentIdx;                                                            //O(1)
                parentIdx = parentIdx / 2;                                                         //O(1)
            }
        }



        // return the top elemenet of the array ( the smallest element ) 
        public vertix_data extractMin()                                             /*    O(log N)    */   // Where N is number of levels
        {
            vertix_data min = Heap[1];                                                  //O(1)
            vertix_data lastNode = Heap[currentSize];                                   //O(1)
            //update the indexes[] and move the last node to the top
            indexes[lastNode.pos] = 1;                                                  //O(1)
            Heap[1] = lastNode;                                                         //O(1)
            Heap[currentSize] = new vertix_data();                                      //O(1)
            HeapDown(1);                                                                //O(log N) 
            currentSize--;                                                              //O(1)
            return min;                                                                 //O(1)
        }


         // update postions of the elements of the Heap 
        public void HeapDown(int k)                                                                    /*      O(log N )      */  // Where N is number of levels
        {
            int smallest = k;                                                                               //O(1)
            int leftChildIdx = 2 * k;                                                                       //O(1)
            int rightChildIdx = 2 * k + 1;                                                                  //O(1)
            if (leftChildIdx < heapSize() && Heap[smallest].weight > Heap[leftChildIdx].weight)             //O(1)
            {
                smallest = leftChildIdx;                                                                    //O(1)
            }
            if (rightChildIdx < heapSize() && Heap[smallest].weight> Heap[rightChildIdx].weight)            //O(1)
            {
                smallest = rightChildIdx;                                                                   //O(1)
            }
            if (smallest != k)                                                                              //O(1)
            {

                vertix_data smallestNode = Heap[smallest];                                                 //O(1)  
                vertix_data kNode = Heap[k];                                                               //O(1)

                //swap
                indexes[smallestNode.pos] = k;                                                             //O(1)
                indexes[kNode.pos] = smallest;                                                             //O(1)
                swap(k, smallest);                                                                         //O(1)
                HeapDown(smallest);                                                                        //O(log N)
            }
        }




         // swap two elements in the Heap 
        public void swap(int first, int second)                  /*      O(1)      */
        {
            vertix_data temp = Heap[first];                            //O(1)
            Heap[first] = Heap[second];                                //O(1)
            Heap[second] = temp;                                       //O(1)
        }


        // check 
        public bool isEmpty()                         /*      O(1)      */
        {
            return currentSize == 0;                        //O(1)
        }



        public int heapSize()                       /*      O(1)      */
        {
            return currentSize;                         //O(1)
        }
    }
}
