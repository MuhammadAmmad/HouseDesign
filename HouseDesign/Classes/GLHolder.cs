using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class GLHolder
    {
        [NonSerialized]
        private OpenGL gl;
        public OpenGL Gl 
        { 
            get
            {
                return gl;
            }
            set
            {
                this.gl = value;
            }
        }

        public GLHolder()
        {

        }
        public GLHolder(OpenGL gl)
        {
            this.Gl = gl;
        }
    }
}
