# Homework2

## 1. Assume you are calling fork in one thread of a multi-thread process in Linux. How many threads will there be in the child process? What problem may be caused by the fork in the child process? Try to explain why Linux have to design fork operation of a multi-thread process like that.

在子进程中只会有一个线程，在[fork(2)-Linux Man Page](https://linux.die.net/man/2/fork)中说明：

> The child process is created with a single thread--the one that called fork(). The entire virtual address space of the parent is replicated in the child, including the states of mutexes, condition variables, and other pthreads objects; the use of pthread_atfork(3) may be helpful for dealing with problems that this can cause.

这种子进程中只剩余调用fork的线程的特性会导致死锁问题。如果父进程中一个线程获取了一个锁，然后另一个线程调用了fork，那么子进程中该锁的持有者就消失掉了（实际上这个锁没有持有者，但是它是被锁上的状态），如果子进程中尝试获取这把锁，那么就会发生死锁。父进程中的线程放锁时并不会影响子进程中锁的状态，因为他们处于两个不同的虚拟地址空间中。

如果fork将父进程中的所有线程都复制的话，需要拷贝所有用户态线程栈和内核态线程栈以及线程本地存储，拷贝的数据会太多导致fork性能太差。

## 2. vfork is a system call similar to fork. Use man vfork to learn more about vfork. Why does vfork perform better than fork in applications consuming a huge amount of memory even when fork already applied the COW policy for memory copying? Try to optimize the performance of fork for such applications without compromising its semantic refers to the idea of vfork.

vfork于fork的区别在于，vfork不对父进程的页表进行复制，而且在子进程终止或调用exec前父进程会被暂停。在需要很大内存的应用中不复制页表进一步减少了swap产生的代价，因此由更好的性能。对fork的改进方法：1、对整个页表都做Copy-On-Write；2、将父进程暂停，知道子进程终止或调用exec。

## 3. In an operating system where a single process contains multiple threads like ChCore, a context switch is usually done at the granularity of thread. However, context switch between threads belonging to the same process can be faster than that between threads belonging to different processes. Why?

同一进程内的线程共享同一地址空间，除线程栈以外的部分全部共享，切换线程栈时不需要刷新TLB；不同进程内的线程切换时，需要切换整个页表，需要刷新TLB，而且由于切换了整个页表，有可能造成更多的缺页异常。

## 4. In ChCore, the page table mapping has been changed during the context switch. However, after the page table changed, the ChCore kernel, which also uses virtual memory address in its code, can still run correctly and get the correct address of its variables and codes. Why?

对于所有进程来说，它们的内核段的页表指向的都是同一段物理地址，所以虽然页表切换了，但是指向内核段的物理地址是相同的，因此内核代码仍然可以正确运行。

## 5. For user-level threads like coroutine, since all coroutines belong to the same thread, only one coroutine can be running at the same time. How can coroutine benefit the performance with the same amount of computing resources? What kind of applications can benefit from coroutine most?

协程在切换上下文时不需要进入内核态，普通的线程上下文切换时必须进入内核态；而却由于协程切换发生在用户态，用户态程序可以根据相应的信息决定下一个调度的协程，线程切换就无法把用户态状态作为参考。生产者-消费者模式的应用可以通过协程提高性能，生产者生产出内容以后直接调度到消费者。

## 6. Now, Assume you are going to create an operating system specialized for the scenario where only one single web server application is running in the OS. The web server requires: 1) Low waiting and processing latency for every user request. 2) Code handling requests from different users can not access each other's memory. How are you going to design the semantic of process/thread in your OS and the program logic of the web server? What should be stored in PCB/TCB, and how is context switch done in your OS?

进程和线程的语义与Linux中相同。Web服务器的编程逻辑是，由一个主线程负责接受请求，接收到请求以后创建子线程处理数据，以此保证比较短的响应时间和处理时间。操作系统在TCB中需要额外保存当前线程的编号，操作系统中每一个页表项要额外记录该页是属于哪一个线程的，以此来保证不同用户间数据的隔离性。操作系统的进程调度会始终将Web服务器进程的优先级放在最高，调度时优先调度。线程调度采用公平调度策略，让每个线程运行时间相近。
