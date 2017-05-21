using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GCS
{

    public static class Geometry
    {
        public static Vector2[] GetIntersect(Shape shape1, Shape shape2)
        {
            if(shape1 is Line)
            {
                if (shape2 is Line)
                    return getIntersect(shape1 as Line, shape2 as Line);
                else if (shape2 is Circle)
                    return getIntersect(shape1 as Line, shape2 as Circle);
            }
            else if(shape1 is Circle)
            {
                if (shape2 is Line)
                    return getIntersect(shape2 as Line, shape1 as Circle);
                else if (shape2 is Circle)
                    return getIntersect(shape1 as Circle, shape2 as Circle);
            }
            throw new Exception("뀨;;");
           
        }
        //교점이 없는 경우 빈 Vector2 배열 리턴
        private static Vector2[] getIntersect(Line line1, Line line2)
        {
           
            if (line1.grad == line2.grad && line1.yint==line2.yint) { throw new NotFiniteNumberException("일치"); }

            else if (line1.grad == line2.grad) { return new Vector2[] { }; }

            float intersectx = (line2.yint - line1.yint) / (line1.grad - line2.grad);
            return new Vector2[] { new Vector2(intersectx, intersectx * line1.grad + line1.yint) };
        }

        private static Vector2[] getIntersect(Line line, Circle circle)
        {
            
          
            
            float [] result= solve2Eq((float)Math.Pow(line.grad,2)+1,
                2*line.grad*(line.yint-circle.Center.Y)-2*circle.Center.X,
                (float)(Math.Pow(circle.Center.X,2)+Math.Pow(line.yint - circle.Center.Y, 2)-Math.Pow(circle.Radius,2)));

            if (result.Length == 0) { return new Vector2[] { }; }
            //교점 없음.
            else if (result.Length == 1) { return new Vector2[] {new Vector2(result[0],line.grad*result[0]+line.yint) }; }
            //교점 1 개
            else
            {
                return new Vector2[] {new Vector2(result[0],line.grad*result[0]+line.yint),
                new Vector2(result[1],line.grad*result[1]+line.yint)};
            }// 교점 2 개
            throw new NotImplementedException();
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="circle1"></param>
        /// <param name="circle2"></param>
        /// <returns></returns>
        private static Vector2[] getIntersect(Circle circle1, Circle circle2)
        {
            float distance = (float)Math.Sqrt(Math.Pow(circle2.Center.X - circle1.Center.X,2) + Math.Pow(circle2.Center.Y - circle1.Center.Y,2));

            if (distance==0) { throw new NotFiniteNumberException("일치"); }
            else if (distance > circle1.Radius + circle2.Radius)//두 원이 밖에 있으면서 만나지 않음.
            {
                return new Vector2[] { };
                
            }
            else if((float)Math.Abs(circle1.Radius-circle2.Radius)>distance)// 두 원 중 하나가 서로를 포함.
            {
                return new Vector2[] { };
            }
            else
            {
                Line Centerline = new Line(circle1.Center, circle2.Center);
                float subgrad = -1 / (Centerline.grad) ;

                Vector2 subpoint = divpoint(
                    circle1.Center,
                    circle2.Center,
                    (cosThm(circle1.Radius,distance, circle2.Radius)[0]*circle1.Radius) / distance,
                    (cosThm(circle1.Radius, distance, circle2.Radius)[1]*circle2.Radius) / distance
                    );

                Line intersectline = new Line(subgrad, subpoint);
                return getIntersect(intersectline, circle1);

            }
            

            throw new NotImplementedException();
        }

        private static float[] solve2Eq(float a, float b, float c)
        {
            if (a == 0) throw new ArgumentException("a는 0이 아님 ㅅㄱ");
            else
            {
                float discriminant = b * b - 4 * a * c;
                if (discriminant < 0) return new float[] { };
                else if (discriminant == 0) { return new float[] { -b / 2 * a }; }
                else
                {
                    return new float[] { (-b + (float)Math.Sqrt(discriminant)) / 2 * a,
                        (-b - (float)Math.Sqrt(discriminant)) / 2 * a };
                    //float 배열 리턴
                }
                
            }
            
        }
        /// <summary>
        /// 존나 섹시함
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>{ cos a&b , cos b&c } 를 리턴</returns>
        private static float[] cosThm(float a, float b, float c) 
        {
            float[] cos2=new float[] {(float)(Math.Pow(c,2)-Math.Pow(a,2)-Math.Pow(b,2))/2*a*b,
            (float)(Math.Pow(a,2)-Math.Pow(b,2)-Math.Pow(a,2))/2*b*c};
            return cos2;
        }

        private static Vector2 divpoint(Vector2 p1, Vector2 p2,float m, float n)//p1과 p2를 m:n으로 내분하는 점의 좌표 리턴
        {
            return new Vector2(m*p2.X+n*p1.X/(m+n), (m*p2.Y+n*p1.Y)/(m+n));
        }
    }
}
