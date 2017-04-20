using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Reflection.Tasks
{
    public class CodeGeneration
    {
        /// <summary>
        /// Returns the functions that returns vectors' scalar product:
        /// (a1, a2,...,aN) * (b1, b2, ..., bN) = a1*b1 + a2*b2 + ... + aN*bN
        /// Generally, CLR does not allow to implement such a method via generics to have one function for various number types:
        /// int, long, float. double.
        /// But it is possible to generate the method in the run time! 
        /// See the idea of code generation using Expression Tree at: 
        /// http://blogs.msdn.com/b/csharpfaq/archive/2009/09/14/generating-dynamic-methods-with-expression-trees-in-visual-studio-2010.aspx
        /// </summary>
        /// <typeparam name="T">number type (int, long, float etc)</typeparam>
        /// <returns>
        ///   The function that return scalar product of two vectors
        ///   The generated dynamic method should be equal to static MultuplyVectors (see below).   
        /// </returns>
        public static Func<T[], T[], T> GetVectorMultiplyFunction<T>() where T : struct 
        {
            var first = Expression.Parameter(typeof(T[]), "first");
            var second = Expression.Parameter(typeof(T[]), "second");
            var result = Expression.Parameter(typeof(T), "result");
            var i = Expression.Parameter(typeof(int), "i");         
            var label = Expression.Label(typeof(T)); 

            var block = Expression.Block
                (
                // Adding local variables.  
                    new[] { result ,i },

                     Expression.Assign(result, Expression.Constant(default(T), typeof(T))),   
                     Expression.Assign(i,Expression.Constant(0)),    

                       // while (true)
                            Expression.Loop
                            (
                                        // {
                                Expression.Block
                                (
                                        // if
                                    Expression.IfThen
                                    (
                                        // (!
                                        Expression.Not
                                        (
                                        // (i < first.Length)
                                            Expression.LessThan
                                            (
                                                i,
                                                Expression.ArrayLength(first)                                                 
                                            )
                                        // )
                                        ),
                                        // break;
                                        Expression.Break(label,result)
                                    ),
                                        // loop body goes here

                                        // result+=
                                        Expression.AddAssign
                                        (
                                            result,
                                            //first[i] * second[i]
                                            Expression.Multiply
                                            (
                                            Expression.ArrayIndex(first,i),
                                            Expression.ArrayIndex(second,i)                                          
                                            )             
                                        ),
            
                                        // i++;
                                    Expression.PostIncrementAssign(i)
                                        // }
                                ),
                                label
                            )
                );   
        
            return Expression.Lambda<Func<T[], T[], T>>(block,first,second).Compile();
        } 
       


        // Static solution to check performance benchmarks
        public static int MultuplyVectors(int[] first, int[] second)
        {
            int result = 0;
            for (int i = 0; i < first.Length; i++) {
                result += first[i] * second[i];
            }
            return result;
        }
    }
}
