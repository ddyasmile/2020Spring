# Homework3 IPC

## 1. Please give a suitable real-world program example for each of the following IPC implementations:

Two processes communicate with each other by polling some data in shared memory

One sender process and one receiver process communicate with each other using non-blocking message passing

One sender process and one receiver process communicate with each other using blocking message passing

One sender process and multiple receiver processes communicate using a shared mailbox

应用规模比较小时，可以采用polling方法，实现简单。

应用允许消息的接受和发送之间存在一定延迟的情况下， 可以使用non-blocking message passing。

应用要求消息的接受和发送之间延迟非常低的情况下， 可以使用blocking message passing。

应用生产者发送信息频率很高，还需要接受消息延迟很低的情况下，可以使用shared mailbox。

## 2. In the Xv6 pipe implementation, what is the usage of the field lock in struct pipe? Why must we put the release/acquire operation inside the sleep function? Why is there no lock operation in the wake function?

锁是为了避免管道读和写同时进行，只有完全写入信息之后才可以读取。在sleep中必须放锁是因为如果不放锁的话，会造成进程a等待进程b填入数据，进程b又等待进程a放锁，导致死锁。weakup中不需要操作锁是因为weakup会回到sleep中，而sleep返回之前还会再次拿锁。

## 3. For shared memory based IPC, there is a typical security problem named "Time-of-check to time-of-use". Please give a brief description of what this problem is with the help of internet and give a possible solution for this problem. HINT: the memory mapping mechanism may help.

对于打开文件这一操作，权限检查和真正打开文件不是同时发生的，如果检查权限通过以后，目标文件重新指向了另一个更高权限的文件，这时就造成了低权限用户打开高权限文件的问题。根本原因是检查和使用不是原子的。解决方法，在检查权限的同时将对应页设为只读，这时有对引用区域的写操作时触发异常，重新进行权限检查。

## 4. For the LRPC implementation, please answer the following questions:

Why is the stack divided into an argument stack and an execution stack?

Where does the overhead come from during the control flow transformation of LRPC?

Are there any security problems caused by the shared argument stack? (Multi-threading are not considered)

因为参数栈和运行栈本质上是属于两个不同的进程的，进程A准备好参数，进程B根据参数来执行。

Overhead有，寻找服务，验证参数，切换页表，修改栈指针。

共享参数栈是安全的，因为页表映射时限制了映射的空间的话，一个进程是无法修改另一个进程上除参数栈以外的部分的。

## 5. Most IPC implementation requires a naming mechanism to find the specific process for communication (e.g., the nread/nwrite pointer in the Xv6 example). Will the design of the naming mechanism dominate the IPC performance? Why?

不会，命名机制只是为了避免轮询浪费资源而使用的唤醒机制的一部分，真正进行进程调度的内核是不受命名机制影响的。消息读、写，进程调度都不受命名机制影响。

## 6. The performance of IPC heavily depends on the hardware architecture (e.g., how the hardware change privilege level and the hardware details of page table switching). For the IPC implementation of ChCore under AArch64 architecture, try to analyze what hardware may influence to the IPC performance. If you are a hardware designer hired by the ChCore team, what modification on the hardware can you do to speed up ChCore's IPC while not compromise security?

由于Chcore中的IPC过程中需要在客户端和服务端之间进行地址映射关系的切换，所以影响IPC性能的硬件是TLB，在修改映射关系之后需要刷新TLB。优化方法是使用ASID，让两个进程切换过程中不需要刷新TLB，提高性能。
