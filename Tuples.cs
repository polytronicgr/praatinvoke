//  
//  Copyright (C) 2009 Geza Kovacs
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;

namespace praatinvoke
{
	public class Pair<T, U>
	{
		public T first;
		public U second;
		public Pair()
		{
			
		}
		public Pair(T f, U s)
		{
			first = f;
			second = s;
		}
	}

	public class Triple<T, U, V>
	{
		public T first;
		public U second;
		public V third;
		public Triple()
		{
			
		}
		public Triple(T f, U s, V t)
		{
			first = f;
			second = s;
			third = t;
		}
	}
}
