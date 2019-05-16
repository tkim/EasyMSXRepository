/* Copyright 2018. Bloomberg Finance L.P.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:  The above
 * copyright notice and this permission notice shall be included in all copies
 * or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */

using Bloomberglp.Blpapi;
using System;
using System.Collections;
using System.Collections.Generic;
using static com.bloomberg.ioiapi.samples.Log;

namespace com.bloomberg.ioiapi.samples
{

    public class Fields : IEnumerable<Field>
    {

        private List<Field> fields = new List<Field>();

        internal IOI ioi;

        internal Fields(IOI ioi)
        {
            this.ioi = ioi;
        }

        internal Field addField(string name, string value)
        {
            Field f =  new Field(this,name, value);
            fields.Add(f);
            return f;

        }

        internal void CurrentToOldValues()
        {

            foreach (Field f in fields)
            {
                f.CurrentToOld();
            }
        }

        public Field field(String name)
        {
            foreach (Field f in fields)
            {
                if (f.Name().Equals(name)) return f;
            }
            return null;
        }

        public IEnumerator<Field> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fields.GetEnumerator();
        }
    }
}