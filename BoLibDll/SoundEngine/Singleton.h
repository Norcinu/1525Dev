#ifndef SINGLETON_H
#define SINGLETON_H

template <class T>

/*
Along with Non-Copyable Singleton allows a class to only be defined once,
and then accessed using the Instance function.
*/

class Singleton
{
public:
	static T* Instance()
	{
		static T t;
		return &t;
	}
};

#endif SINGLETON_H