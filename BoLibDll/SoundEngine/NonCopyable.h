#ifndef NONCOPYABLE_H
#define NONCOPYABLE_H

/*
Inherit from this class to make an object non-copyable, used mainly for singleton classes
*/

class NonCopyable
{
protected:
	NonCopyable(){}
private:
	NonCopyable(const NonCopyable&);
	NonCopyable& operator=(const NonCopyable&);
};

#endif NONCOPYABLE_H