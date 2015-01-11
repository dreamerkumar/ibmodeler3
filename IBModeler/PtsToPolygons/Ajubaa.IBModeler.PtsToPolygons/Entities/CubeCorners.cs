namespace Ajubaa.IBModeler.PtsToPolygons
{
    public class CubeCorners
    {
        public bool BackTopLeft { get; set; }
        public bool BackBottomLeft { get; set; }
        public bool BackBottomRight { get; set; }
        public bool BackTopRight { get; set; }
        public bool FrontTopLeft { get; set; }
        public bool FrontBottomLeft { get; set; }
        public bool FrontBottomRight { get; set; }
        public bool FrontTopRight { get; set; }

        public CubeCorners()
        {
            //Set all the flags to zero
            BackTopLeft = false;
            BackBottomLeft = false;
            BackBottomRight = false;
            BackTopRight = false;
            FrontTopLeft = false;
            FrontBottomLeft = false;
            FrontBottomRight = false;
            FrontTopRight = false;
        }
        public bool TriangleOnCubeFace()
        {
            //If three points are on one face, then the other face will be blank
            return
                //left face
                (!BackBottomLeft && !BackTopLeft && !FrontBottomLeft && !FrontTopLeft)
                //right face
                || (!BackBottomRight && !BackTopRight && !FrontBottomRight && !FrontTopRight)
                //front face
                || (!FrontBottomLeft && !FrontBottomRight && !FrontTopLeft && !FrontTopRight)
                //back face
                || (!BackBottomLeft && !BackBottomRight && !BackTopLeft && !BackTopRight)
                //top face
                || (!BackTopLeft && !BackTopRight && !FrontTopLeft && !FrontTopRight)
                //bottom face
                || (!BackBottomLeft && !BackBottomRight && !FrontBottomLeft && !FrontBottomRight);
        }
    }
}
