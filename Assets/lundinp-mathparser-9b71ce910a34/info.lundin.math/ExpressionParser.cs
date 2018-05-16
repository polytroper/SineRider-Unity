
// Author: Patrik Lundin, patrik@lundin.info, http://www.lundin.info

namespace info.lundin.math
{

	using System;
	using System.IO;
	using System.Text;
	using System.Collections;

	/// <summary>
	///
	///	Class ExpressionParser, this class evaluates a mathematical expression given 
	///	as a String to a double value.	
	///	Author: Patrik Lundin, patrik@lundin.info, http://www.lundin.info
	///	
	///	
	///	Patrik Lundin, patrik@lundin.info, http://www.lundin.info
	/// Copyright 2002-2012 Patrik Lundin
	///	
	///	This file is part of info.lundin.Math.
	///	
	/// This library is free software; you can redistribute it and/or
	/// modify it under the terms of the GNU Lesser General Public
	/// License as published by the Free Software Foundation; either
	/// version 2.1 of the License, or (at your option) any later version.
	/// 
	/// This library is distributed in the hope that it will be useful,
	/// but WITHOUT ANY WARRANTY; without even the implied warranty of
	/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	/// Lesser General Public License for more details.
	/// 
	/// You should have received a copy of the GNU Lesser General Public
	/// License along with this library; if not, write to the Free Software
	/// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
	///	
	/// </summary>
	public class ExpressionParser
    {

        #region Private Member Variables

        private Hashtable ops, trees, htbl, spconst;
		private int maxoplength;
		private int sb_init;
        bool bRequireParentheses;
        bool bImplicitMultiplication;

        #endregion

        #region Constructors

        /// <summary>
		/// Default constructor, creates an ExpressionParser object
		/// </summary>
		public ExpressionParser()
		{
			ops   = new Hashtable( 52 ); // Holds operators
			spconst = new Hashtable( 12 ); // Holds constants
			trees = new Hashtable( 101 ); // Holds Node tree datastructures
			
			// Add all valid operators.
			// new Operator( operator, arguments, precedence )
			// 
			// To add a new operator to the parser three things need to be added:
			//
			// 1. A new line below specifying the operator symbol, the number of
			// arguments (in this parser max two arguments!) and the operator precedence.
			//
			// 2. Change the maxoplength below (if needed) to hold the number of characters
			// an operator symbol length can be.
			//
			// 3. Add the code to evaluate the operator inside the toValue method using the
			// same recursive calls as for the other operators.
			//
			ops.Add( "^",     new Operator( "^",	2, 3 ) );
			ops.Add( "+",     new Operator( "+",	2, 6 ) );
			ops.Add( "-",     new Operator( "-",	2, 6 ) );
			ops.Add( "/", 	  new Operator( "/",	2, 4 ) );
			ops.Add( "*",     new Operator( "*",	2, 4 ) );
			ops.Add( "cos",   new Operator( "cos",	1, 2 ) );
			ops.Add( "sin",   new Operator( "sin",	1, 2 ) );
			ops.Add( "exp",   new Operator( "exp",	1, 2 ) );
			ops.Add( "ln",    new Operator( "ln",	1, 2 ) );
			ops.Add( "tan",   new Operator( "tan",	1, 2 ) );
			ops.Add( "acos",  new Operator( "acos",	1, 2 ) );
			ops.Add( "asin",  new Operator( "asin",	1, 2 ) );
			ops.Add( "atan",  new Operator( "atan",	1, 2 ) );
			ops.Add( "cosh",  new Operator( "cosh",	1, 2 ) );
			ops.Add( "sinh",  new Operator( "sinh",	1, 2 ) );
			ops.Add( "tanh",  new Operator( "tanh",	1, 2 ) );
			ops.Add( "sqrt",  new Operator( "sqrt",	1, 2 ) );
			ops.Add( "cotan", new Operator( "cotan",1, 2 ) );
			ops.Add( "fpart", new Operator( "fpart",1, 2 ) );
			ops.Add( "acotan",new Operator( "acotan",1, 2 ) );
			ops.Add( "round", new Operator( "round", 1, 2 ) );
			ops.Add( "ceil",  new Operator( "ceil",  1, 2 ) );
			ops.Add( "floor", new Operator( "floor",1, 2 ) );
			ops.Add( "fac",	  new Operator( "fac",	1, 2 ) );
			ops.Add( "sfac",  new Operator( "sfac",	1, 2 ) );
			ops.Add( "abs",	  new Operator( "abs",	1, 2 ) );
			ops.Add( "log",	  new Operator( "log",	1, 2 ) );
			ops.Add( "%",     new Operator( "%",	2, 4 ) );
			ops.Add( ">",     new Operator( ">",	2, 7 ) );
			ops.Add( "<",     new Operator( "<",	2, 7 ) );
			ops.Add( "&&",    new Operator( "&&",	2, 8 ) );
			ops.Add( "==",    new Operator( "==",	2, 7 ) );
			ops.Add( "!=",    new Operator( "!=",	2, 7 ) );
			ops.Add( "||",    new Operator( "||",	2, 9 ) );
			ops.Add( "!",     new Operator( "!",	1, 1 ) );
			ops.Add( ">=",    new Operator( ">=",	2, 7 ) );
			ops.Add( "<=",    new Operator( "<=" ,	2, 7 ) );
			
			// Constants
			spconst.Add( "euler",     Math.E  );
			spconst.Add( "pi" ,       Math.PI );
			spconst.Add( "nan" ,      double.NaN );
			spconst.Add( "infinity" , double.PositiveInfinity );
			spconst.Add( "true" ,    1D  );
			spconst.Add( "false",    0D );
					
			// maximum operator length, used when parsing.
			maxoplength = 6;
			
			// init all StringBuilders with this value.
			// this will be set to the length of the expression being evaluated by Parse.
			sb_init = 50;

            bRequireParentheses = true;
            bImplicitMultiplication = true;
		}

        #endregion

        #region Public Properties

        /// <summary>
        /// Enables or disables the requirement to put all function arguments within parantheses.
        /// 
        /// Default is true making it required for all function arguments to be enclosed within () for example sin(x+5)
        /// failure to do so will generate an exception stating that parantheses are required.
        /// 
        /// Setting this property to false disables this requirement making expressions such as for example sinx or sin5 allowed.
        /// </summary>
        public bool RequireParentheses
        {
            get
            {
                return bRequireParentheses;
            }
            set
            {
                bRequireParentheses = value;
            }
        }

        /// <summary>
        /// Enables or disables the support for implicit multiplication.
        /// 
        /// Default is true making implicit multiplication allowed, for example 2x or 3sin(2x)
        /// setting this property to false disables support for implicit multiplication making the * operator required
        /// for example 2*x or 3*sin(2*x)
        /// 
        /// If implicit multiplication is disabled (false) parsing an expression that does not explicitly use the * operator may
        /// throw syntax errors with various error messages.
        /// </summary>
        public bool ImplicitMultiplication
        {
            get
            {
                return bImplicitMultiplication;
            }
            set
            {
                bImplicitMultiplication = value;
            }
        }

        #endregion

        #region Public Methods



        /// <summary>
        /// Evaluates the infix expression using the values in the Hashtable.
        /// </summary>
        /// <remarks>
        /// This is the only publicly available method of the class, it is the entry point into for the user 
        /// into the parser.
        ///
        /// Example usage:
        /// 
        /// using info.lundin.Math;
        /// using System;
        /// using System.Collections;
        ///
        /// public class Test 
        /// {
        /// 	public static void Main( string[] args )
        /// 	{
        /// 		ExpressionParser parser = new ExpressionParser();
        /// 		Hashtable h = new Hashtable();
        ///
        /// 		h.Add( "x", 1.ToString() );
        /// 		h.Add( "y", 2.ToString() );
        ///
        ///
        /// 		double result = parser.Parse( "xcos(y)", h );
        /// 		Console.WriteLine( "Result: {0}", result );
        /// 	}
        /// }
        ///
        /// </remarks>
        /// <param name="exp">the infix string expression to parse and evaluate.</param>
        /// <param name="tbl">Hashtable with variable value pairs</param>
        /// <returns>a double value</returns>
        public double
        Parse(String exp, Hashtable tbl)
        {
            double ans = 0D;
            String tmp;
            Node tree;

            if (exp == null || exp.Equals(""))
            {
                throw new ParserException("First argument to method Parse is null or empty");
            }
            else if (tbl == null)
            {
                return Parse(exp, new Hashtable());
            }

            this.htbl = tbl;
            tmp = skipSpaces(exp.ToLower());
            this.sb_init = tmp.Length;

            try
            {
                if (trees.ContainsKey(tmp))
                {
                    ans = toValue((Node)trees[tmp]);
                }
                else
                {
                    Syntax(tmp);
                    tree = parse(putMult(parseE(tmp)));
                    ans = toValue(tree);
                    trees.Add(tmp, tree);
                }

                return ans;
            }
            catch (Exception e)
            {
                throw new ParserException(e.Message);
            }
        }

        public double
        Parse(String exp)
        {
            return Parse(exp, null);
        }

        #endregion

        #region All Private Methods

        #region Private, Methods for preparsing infix expression

        /// <summary>
		/// Checks the String expression to see if the syntax is valid.
		/// this method doesn't return anything, instead it throws an Exception
		/// if the syntax is invalid.
		///	
		/// Examples of invalid syntax can be non matching paranthesis, non valid symbols appearing
		/// or a variable or operator name is invalid in the expression.
		/// </summary>
		/// <param name="exp">the string expression to check, infix notation.</param>
        /// 
        /// <remarks>This validates some syntax errors such as unbalanced paranthesis and invalid characters.
        /// Exceptions can also be thrown at evaluation time.</remarks>
		private void 
		Syntax( String exp )
		{
		  int i = 0, oplen = 0;
		  String op = null;
		  String nop = null;  
		
            // Check if all paranthesis match in expression
		  if( ! matchParant(exp) )
		  {
              throw new ParserException("Unbalanced parenthesis");
		  }
		
		  int l = exp.Length;
		
          // Go through expression and validate syntax
		  while( i < l )
		  {
		    try
            {

		      if( ( op = getOp( exp , i ) ) != null)
		      {
                // Found operator at position, check syntax for operators

				oplen = op.Length;
				i += oplen;

                // If it's a function and we are missing parentheses around arguments and bRequireParantheses is true it is an error.
                // Note that this only checks opening paranthesis, we checked whole expression for balanced paranthesis
                // earlier but not for each individual function.
                if (bRequireParentheses && !isTwoArgOp(op) && op != "!" && exp[i] != '(')
                {
                    throw new ParserException("Paranthesis required for arguments -> " + exp.Substring(i - oplen));
                }

                // If we have an operator immediately following a function and it's not unary + or - then it's an error
                nop = getOp( exp, i );
				if( nop != null && isTwoArgOp( nop ) && ! ( nop.Equals("+") || nop.Equals("-") ) )
				{
					throw new ParserException( "Syntax error near -> " + exp.Substring( i - oplen ) );
				}
		      }
			  else if( ! isAlpha( exp[i] ) && ! isConstant( exp[i] ) && ! isAllowedSym( exp[i] ) )
			  {
                  // This cannot be a valid character, throw exception
				  throw new ParserException( "Syntax error near -> " + exp.Substring( i ) );
			  }
		      else
		      {
                  // Count forward
				i++;
		      }
		    }	
		    catch( IndexOutOfRangeException )
		    {
                // Might happen when we are checking syntax, just move forward
		      i++;
		    }
		  }
		
		  return;
		}
		


		/// <summary>
		/// Inserts the multiplication operator where needed.
		/// This method adds limited support for implicit multiplication in expressions.
		/// </summary>
		/// <remarks>	
		/// Implicit multiplication is supported in these type cases:
		///
		/// case: variable jp one-arg-op , xcos(x)
		/// case: const jp variable or one-arg-op, 2x, 2tan(x)
		/// case: "const jp ( expr )" , 2(3+x)
		/// case: ( expr ) jp variable or one-arg-op , (2-x)x , (2-x)sin(x)
		/// case: var jp  ( expr ) , x(x+1) , x(1-sin(x))
		///
		/// Note that this also puts extra limitations on variable names, they cannot
		/// contain digits within them or at the beginning, only at the end.
        /// 
        /// If the ImplicitMultiplication property is set to false this method just returns
        /// the same expression sent to it without modification, effectively disabling support
        /// for implicit multiplication. This hoewever does not change the variable naming requirements.
		/// </remarks>
		/// <param name="exp">the infix string expression to process</param>
		/// <returns>the processed infix expression</returns>
		private String
		putMult( String exp )
		{
            // Return expression unchanged if ImplicitMultiplication property is false.
            if (!bImplicitMultiplication) return exp;

            int i = 0, p = 0;
            String tmp = null;
            StringBuilder str = new StringBuilder( exp );

            int l = exp.Length;

            while (i < l)
            {
                try
                {
                    if ((tmp = getOp(exp, i)) != null && !isTwoArgOp(tmp) && (i - 1 >= 0 && isAlpha(exp[i - 1])))
                    {
                        // case: variable jp one-arg-op , xcos(x)
                        str.Insert(i + p, "*");
                        p++;
                    }
                    else if (isAlpha(exp[i]) && (i - 1 >= 0 && isConstant(exp[i - 1])))
                    {
                        // case: const jp variable or one-arg-op, 2x, 2tan(x)
                        str.Insert(i + p, "*");
                        p++;
                    }
                    else if (exp[i] == '(' && (i - 1 >= 0 && isConstant(exp[i - 1])))
                    {
                        // case: "const jp ( expr )" , 2(3+x)
                        str.Insert(i + p, "*");
                        p++;
                    }
                    else if (isAlpha(exp[i]) && (i - 1 >= 0 && exp[i - 1] == ')'))
                    {
                        // case: ( expr ) jp variable or one-arg-op , (2-x)x , (2-x)sin(x)
                        str.Insert(i + p, "*");
                        p++;
                    }
                    else if (exp[i] == '(' && (i - 1 >= 0 && exp[i - 1] == ')'))
                    {
                        // case: ( expr ) jp  ( expr ) , (2-x)(x+1) , sin(x)(2-x) 
                        str.Insert(i + p, "*");
                        p++;
                    }
                    else if (exp[i] == '(' && (i - 1 >= 0 && isAlpha(exp[i - 1])) && backTrack(exp.Substring(0, i)) == null)
                    {
                        // case: var jp  ( expr ) , x(x+1) , x(1-sin(x))
                        str.Insert(i + p, "*");
                        p++;
                    }
                }
                catch
                {
                    // checking indexes properly so try/catch should not be needed but lets keep it for now
                }

                if (tmp != null)
                {
                    i += tmp.Length;
                }
                else
                {
                    i++;
                }

                tmp = null;
            }

            return str.ToString();
		}


		/// <summary>
		/// Adds support for "scientific notation" by replacing the E operator with *10^
		/// </summary>
		/// <remarks>
		/// For example the value 1E-3 would be changed to 1*10^-3 which the parser will treat
		/// as a normal expression.
		/// </remarks>
		/// <param name="exp">the infix string expression to process</param>
		/// <returns>the processed infix expression</returns>
		private String
		parseE( String exp )
		{
		 
		  int i, p , len;
		
		  StringBuilder newstr = new StringBuilder( exp );
		
		  i = p = 0;
		  len = exp.Length;
		
		  while( i < len )
		  {
		   try
           {
		    if( exp[i] == 'e' && ( i-1 >= 0 && Char.IsDigit(exp[i-1])))
		    {
			    if( (i+1 < len && Char.IsDigit(exp[i+1])) || (i+2 < len && ((exp[i+1] == '-' || exp[i+1] == '+') && Char.IsDigit(exp[i+2]))))
			    {
			      // replace the 'e'
			      newstr[ i + p ] =  '*';
			      // insert the rest
			      newstr.Insert( i + p + 1 , "10^" );
			      p = p + 3; // buffer growed by 3 chars
			    }
		     }
		   }
           catch
           {
               // checking indexes properly so try/catch should not be needed but lets keep it for now
           }

		   i++;
		  }
		  
		  return newstr.ToString();
		}

        #endregion

        #region Private, Core Parser Methods

        /// <summary>
        /// Parses an infix String expression and creates a parse tree of Node's.
        /// </summary>
        /// <remarks>
        /// This is the heart of the parser, it takes a normal infix expression and creates
        /// a tree datastructure we can easily recurse when evaluating.
        ///
        /// The tree datastructure is then evaluated by the toValue method.
        /// </remarks>
        /// <param name="exp">the infix string expression to process</param>
        /// <returns>A tree datastructure of Node objects representing the expression</returns>
        private Node
        parse(String exp)
        {
            int i, ma, len;
            String farg, sarg, fop;
            Node tree = null;

            farg = sarg = fop = "";
            ma = i = 0;

            len = exp.Length;

            if (len == 0)
            {
                throw new ParserException("Wrong number of arguments to operator");
            }
            else if (exp[0] == '(' && ((ma = match(exp, 0)) == (len - 1)))
            {
                return (parse(exp.Substring(1, ma - 1)));
            }
            else if (isVariable(exp))
            {
                // If built in constant put in value otherwise the variable
                if (spconst.ContainsKey(exp)) return new Node((double)spconst[exp]);
                else return new Node(exp);
            }
            else if (isAllNumbers(exp))
            {
                try
                {
                    return (new Node(Double.Parse(exp)));
                }
                catch (FormatException)
                {
                    throw new ParserException("Syntax error-> " + exp + " (not using regional decimal separator?)");
                }
            }

            while (i < len)
            {
                if ((fop = getOp(exp, i)) == null)
                {
                    farg = arg(null, exp, i);
                    fop = getOp(exp, i + farg.Length);

                    if (fop == null) throw new Exception("Missing operator");

                    if (isTwoArgOp(fop))
                    {
                        sarg = arg(fop, exp, i + farg.Length + fop.Length);
                        if (sarg.Equals("")) throw new Exception("Wrong number of arguments to operator " + fop);
                        tree = new Node(fop, parse(farg), parse(sarg));
                        i += farg.Length + fop.Length + sarg.Length;
                    }
                    else
                    {
                        if (farg.Equals("")) throw new Exception("Wrong number of arguments to operator " + fop);
                        tree = new Node(fop, parse(farg));
                        i += farg.Length + fop.Length;
                    }
                }
                else
                {
                    if (isTwoArgOp(fop))
                    {
                        farg = arg(fop, exp, i + fop.Length);
                        if (farg.Equals("")) throw new Exception("Wrong number of arguments to operator " + fop);
                        if (tree == null)
                        {
                            if (fop.Equals("+") || fop.Equals("-"))
                            {
                                tree = new Node(0D);
                            }
                            else throw new Exception("Wrong number of arguments to operator " + fop);
                        }
                        tree = new Node(fop, tree, parse(farg));
                        i += farg.Length + fop.Length;
                    }
                    else
                    {
                        farg = arg(fop, exp, i + fop.Length);
                        if (farg.Equals("")) throw new Exception("Wrong number of arguments to operator " + fop);
                        tree = new Node(fop, parse(farg));
                        i += farg.Length + fop.Length;
                    }
                }

            }

            return tree;
        }

        /// <summary>
        /// Traverses and evaluates the datastructure created by the parse method.
        /// </summary>
        /// <remarks>
        /// This is where the actual evaluation of the expression is made,
        /// the Node tree structure created by the parse method is traversed and evaluated.
        /// </remarks>
        /// <param name="tree">A Node representing a tree datastructure</param>
        /// <returns>A double value</returns>
        private double
        toValue(Node tree)
        {
            Node arg1, arg2;
            double val;
            String op, tmp, var;

            if (tree.getType() == Node.TYPE_CONSTANT)
            {
                return (tree.getValue());
            }
            else if (tree.getType() == Node.TYPE_VARIABLE)
            {
                var = tree.getVariable();

                // get value associated with variable
                tmp = get(var);

                if (trees.ContainsKey(tmp)) // cached expression
                {
                    return toValue((Node)trees[tmp]);
                }
                else if (isConstant(tmp)) // constant value
                {
                    return (Double.Parse(tmp));
                }
                else
                {
                    // apparently a nested expression, parse and cache

                    /*
                     * Possible circle detection algorithm would be to use a hashtable, then first check if the variable we have is in use (in hashtable),
                     * if not we put in the variable we have into the hashtable and proceed. If it is being used (is in hashtable) we have a circular condition.
                     * Finally if we were able to continue, after the toValue(tree) we remove the variable from the hashtable so it is flagged as not being used.
                     * Problem is the performance of these checks.
                     * */

                    tmp = skipSpaces(tmp.ToLower());
                    htbl[var] = tmp;

                    Syntax(tmp);

                    tree = parse(putMult(parseE(tmp)));
                    trees.Add(tmp, tree);

                    return toValue(tree);
                }
            }

            op = tree.getOperator();
            arg1 = tree.arg1();

            if (tree.arguments() == 2)
            {
                arg2 = tree.arg2();

                if (op.Equals("+"))
                    return (toValue(arg1) + toValue(arg2));
                else if (op.Equals("-"))
                    return (toValue(arg1) - toValue(arg2));
                else if (op.Equals("*"))
                    return (toValue(arg1) * toValue(arg2));
                else if (op.Equals("/"))
                    return (toValue(arg1) / toValue(arg2));
                else if (op.Equals("^"))
                    return (Math.Pow(toValue(arg1), toValue(arg2)));
                else if (op.Equals("%"))
                    return (toValue(arg1) % toValue(arg2));
                else if (op.Equals("=="))
                    return (toValue(arg1) == toValue(arg2) ? 1.0 : 0.0);
                else if (op.Equals("!="))
                    return (toValue(arg1) != toValue(arg2) ? 1.0 : 0.0);
                else if (op.Equals("<"))
                    return (toValue(arg1) < toValue(arg2) ? 1.0 : 0.0);
                else if (op.Equals(">"))
                    return (toValue(arg1) > toValue(arg2) ? 1.0 : 0.0);
                else if (op.Equals("&&"))
                    return ((toValue(arg1) == 1.0) && (toValue(arg2) == 1.0) ? 1.0 : 0.0);
                else if (op.Equals("||"))
                    return ((toValue(arg1) == 1.0) || (toValue(arg2) == 1.0) ? 1.0 : 0.0);
                else if (op.Equals(">="))
                    return (toValue(arg1) >= toValue(arg2) ? 1.0 : 0.0);
                else if (op.Equals("<="))
                    return (toValue(arg1) <= toValue(arg2) ? 1.0 : 0.0);
            }
            else
            {
                if (op.Equals("sqrt"))
                    return (Math.Sqrt(toValue(arg1)));
                else if (op.Equals("sin"))
                    return (Math.Sin(toValue(arg1)));
                else if (op.Equals("cos"))
                    return (Math.Cos(toValue(arg1)));
                else if (op.Equals("tan"))
                    return (Math.Tan(toValue(arg1)));
                else if (op.Equals("asin"))
                    return (Math.Asin(toValue(arg1)));
                else if (op.Equals("acos"))
                    return (Math.Acos(toValue(arg1)));
                else if (op.Equals("atan"))
                    return (Math.Atan(toValue(arg1)));
                else if (op.Equals("ln"))
                    return (Math.Log(toValue(arg1)));
                else if (op.Equals("log"))
                    return (Math.Log10(toValue(arg1)));
                else if (op.Equals("exp"))
                    return (Math.Exp(toValue(arg1)));
                else if (op.Equals("cotan"))
                    return (1 / Math.Tan(toValue(arg1)));
                else if (op.Equals("acotan"))
                    return (Math.PI / 2 - Math.Atan(toValue(arg1)));
                else if (op.Equals("ceil"))
                    return ((double)Math.Ceiling(toValue(arg1)));
                else if (op.Equals("round"))
                    return ((double)Math.Round(toValue(arg1)));
                else if (op.Equals("floor"))
                    return ((double)Math.Floor(toValue(arg1)));
                else if (op.Equals("fac"))
                    return (fac(toValue(arg1)));
                else if (op.Equals("abs"))
                    return (Math.Abs(toValue(arg1)));
                else if (op.Equals("fpart"))
                    return (fpart(toValue(arg1)));
                else if (op.Equals("sfac"))
                    return (sfac(toValue(arg1)));
                else if (op.Equals("sinh"))
                {
                    val = toValue(arg1);
                    return ((Math.Exp(val) - (1 / Math.Exp(val))) / 2);
                }
                else if (op.Equals("cosh"))
                {
                    val = toValue(arg1);
                    return ((Math.Exp(val) + (1 / Math.Exp(val))) / 2);
                }
                else if (op.Equals("tanh"))
                {
                    val = toValue(arg1);
                    return (((Math.Exp(val) - (1 / Math.Exp(val))) / 2) / ((Math.Exp(val) + (1 / Math.Exp(val))) / 2));
                }
                else if (op.Equals("!"))
                {
                    return ((!(toValue(arg1) == 1.0)) ? 1.0 : 0.0);
                }
            }

            throw new ParserException("Unknown operator");
        }

        #endregion

        #region Private, Helper Methods

        /// <summary>Matches all paranthesis and returns true if they all match or false if they do not.</summary>
        /// <param name="exp">expression to check, infix notation</param>
        /// <returns>true if ok false otherwise</returns>
        private bool
        matchParant(String exp)
        {
            int count = 0;
            int i = 0;

            int l = exp.Length;

            for (i = 0; i < l; i++)
            {
                if (exp[i] == '(')
                {
                    count++;
                }
                else if (exp[i] == ')')
                {
                    count--;
                }
            }

            return (count == 0);
        }



        /// <summary>Checks if the character is alphabetic.</summary>
        /// <param name="ch">Character to check</param>
        /// <returns>true or false</returns>
        private bool
        isAlpha(char ch)
        {
            return ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'));
        }



        /// <summary>Checks if the string can be considered to be a valid variable name.</summary>
        /// <param name="str">The String to check</param>
        /// <returns>true or false</returns>
        private bool
        isVariable(String str)
        {
            int i = 0;
            int len = str.Length;

            if (isAllNumbers(str)) return false;

            for (i = 0; i < len; i++)
            {
                if (getOp(str, i) != null || isAllowedSym(str[i]))
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>Checks if the character is a digit</summary>
        /// <param name="ch">Character to check</param>
        /// <returns>true or false</returns>
        private bool
        isConstant(char ch)
        {
            return (Char.IsDigit(ch));
        }



        /// <summary>Checks to se if a string is numeric</summary>
        /// <param name="exp">String to check</param>
        /// <returns>true if the string was numeric, false otherwise</returns>
        private bool
        isConstant(String exp)
        {
            double val = 0D;
            bool ok = Double.TryParse(exp, out val);
            return (ok && !Double.IsNaN(val));
        }


        /// <summary>
        /// Checks to see if this String consists of only digits and punctuation.
        /// </summary>
        /// <remarks>
        /// Probably not needed in .NET at all. This is actually a legacy from the Java version
        /// where it was needed because some older JVM's accepted strings that started
        /// with digits as numeric when the isConstant method was used.
        /// </remarks>
        /// <param name="str">The string to check</param>
        /// <returns>true if the string was numeric, false otherwise.</returns>
        private bool
        isAllNumbers(String str)
        {
            char ch;
            int i = 0, l = 0;
            bool dot = false;

            ch = str[0];

            if (ch == '-' || ch == '+') i = 1;

            l = str.Length;

            while (i < l)
            {
                ch = str[i];

                if (!(Char.IsDigit(ch) || ((ch == '.' || ch == ',') && !dot)))
                {
                    return false;
                }

                dot = (ch == '.' || ch == ',');

                i++;
            }

            return true;
        }


        /// <summary>
        /// Checks to see if the string is the name of a acceptable operator.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>true if it is an acceptable operator, false otherwise.</returns>
        private bool
        isOperator(String str)
        {
            return (ops.ContainsKey(str));
        }



        /// <summary>
        /// Checks to see if the operator name represented by str takes two arguments.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>true if the operator takes two arguments, false otherwise.</returns>
        private bool
        isTwoArgOp(String str)
        {
            if (str == null) return false;
            Object o = ops[str];
            if (o == null) return false;
            return (((Operator)o).arguments() == 2);
        }


        /// <summary>
        /// Checks to see if the double value a can be considered to be a mathematical integer.</summary>
        /// <remarks>
        /// This method is only used by the fac and sfac methods and not the parser itself, it should
        /// really leave this class since they have nothing to do with the parser.
        /// </remarks>
        /// <param name="a">the double value to check</param>
        /// <returns>true if the double value is an integer, false otherwise.</returns>
        private bool
        isInteger(double a)
        {
            return ((a - (int)a) == 0.0);
        }


        /// <summary>
        /// Checks to see if the int value a can be considered to be even. </summary>
        /// <remarks>
        /// This method is only used by the fac and sfac methods and not the parser itself, it should
        /// really leave this class since they have nothing to do with the parser.
        /// </remarks>
        /// <param name="a">the int value to check</param>
        /// <returns>true if the int value is even, false otherwise.</returns>
        private bool
        isEven(int a)
        {
            return (isInteger(a / 2));
        }


        /// <summary>
        /// Checks to see if the character is a valid symbol for this parser.
        /// </summary>
        /// <param name="s">the character to check</param>
        /// <returns>true if the char is valid, false otherwise.</returns>
        private bool
        isAllowedSym(char s)
        {
            return (s == ',' || s == '.' || s == ')' || s == '(' || s == '>' || s == '<' || s == '&' || s == '=' || s == '|');
        }

        /// <summary>
		/// Parses out spaces from a string
		/// </summary>
		/// <param name="str">The string to process</param>
		/// <returns>A copy of the string stripped of all spaces</returns>
		private String
		skipSpaces( String str  )
		{
		  int i = 0;
		  int len = str.Length;
		  StringBuilder nstr = new StringBuilder( len );
		  
		  while( i < len )
		  {
			if( str[ i ] != ' ' )
			{
				nstr.Append( str[i] );
			}
			i++;
		  }
		
		  return nstr.ToString();
		}
		


		/// <summary>
		/// Matches an opening left paranthesis.
		/// </summary>
		/// <param name="exp">the string to search in</param>
		/// <param name="index">the index of the opening left paranthesis</param>
		/// <returns>the index of the matching closing right paranthesis</returns>
		private int
		match( String exp,int index )
		{
		  int len = exp.Length;
		  int i = index;
		  int count = 0;
		
		  while( i < len )
		  {
		    if( exp[i] == '(')
		    {
		      count++;
		    }
		    else if( exp[i] == ')')
		    {
		      count--;
		    }
		
		    if( count == 0 ) return i;
		
		    i++;
		  }
		  
		  return index;
		}
		
		/// <summary>
		/// Parses out an operator from an infix string expression.
		/// </summary>
		/// <param name="exp">the infix string expression to look in</param>
		/// <param name="index">the index to start searching from</param>
		/// <returns>the operator if any or null.</returns>
		private String 
		getOp( String exp , int index )
		{
		  String tmp; 
		  int i = 0;
		  int len = exp.Length;
		  
		  for( i = 0 ; i < maxoplength ; i++ )
		  {
		    if( index >= 0 && ( index + maxoplength - i ) <= len )
		    {
		      tmp = exp.Substring( index ,  maxoplength - i  );
		      if( isOperator( tmp ) )
		      {
                return( tmp );
		      }
		    }
		  }
		
		  return null;
		}

		/// <summary>
		/// Parses the infix expression for arguments to the specified operator.
		/// </summary>
		/// <param name="_operator">the operator we are interested in</param>
		/// <param name="exp">the infix string expression</param>
		/// <param name="index">the index to start the search from</param>
		/// <returns>the argument to the operator</returns>
		private String
		arg( String _operator, String exp, int index )
		{
		  int ma, i, prec = -1;
		  int len = exp.Length;
		  String op = null;
		 
		  StringBuilder str = new StringBuilder( sb_init );
		
		  i = index;
		  ma = 0;
		
		  if( _operator == null )
		  {
			prec = -1;
		  }
		  else
		  {
			prec = ((Operator)ops[ _operator ]).precedence();	
		  }

		  while( i < len )
		  {
			  
		    if( exp[i] == '(')
		    {
		      ma = match( exp, i );
		      str.Append( exp.Substring( i , ma + 1 - i ));
		      i = ma + 1;
		    }
		    else if( ( op = getOp( exp, i )) != null )
		    {
			// (_operator != null && _operator.Equals("&&") && op.Equals("||") ) || 
		      if( str.Length != 0 && ! isTwoArgOp( backTrack( str.ToString() )) && ((Operator)ops[ op ]).precedence() >= prec )
		      {
				return str.ToString();
		      }
		      str.Append( op );
		      i += op.Length;
		    }
		    else
		    {
		      str.Append( exp[i] );
		      i++;
		    }
		  }

		  return str.ToString();
		}
			


		/// <summary>
		/// Returns an operator at the end of the String str if present.
		/// </summary>
		/// <remarks>
		/// Used when parsing for arguments, the purpose is to recognize
		/// expressions like for example 10^-1
		/// </remarks>
		/// <param name="str">part of infix string expression to search</param>
		/// <returns>the operator if found or null otherwise</returns>
		private String
		backTrack( String str  )
		{
		  int i = 0;
		  int len = str.Length;
		  String op = null;
		
		  try{
		    for( i = 0; i <= maxoplength ; i++  ){
		      if( ( op = getOp( str , ( len - 1 - maxoplength + i ))) != null 
			 && ( len - maxoplength - 1 + i + op.Length ) == len )
		      {
			return op;
		      }
		    }
		  }catch{}
		
		  return null;
		}

		/// <summary>
		/// Retrieves a value stored in the Hashtable containing all variable = value pairs.
		/// </summary>
		/// <remarks>
		/// The hashtable used in this method is set by the Parse( String, Hashtable ) method so this method retrives
		/// values inserted by the user of this class. Please note that no processing has been made
		/// on these values, they may have incorrect syntax or casing.
		/// </remarks>
		/// <param name="key">the name of the variable we want the value for</param>
		/// <returns>the value stored in the Hashtable or null if none.</returns>
		private String
		get( String key ) 
		{
		  Object ob  = this.htbl[key];
		  String val = null;

		  if( ob == null  ) throw new ParserException("No value associated with " + key);
		  
		  try{
			val = (String)ob;
		  }
		  catch
		  {
			 throw new ParserException("Wrong type value for " + key + " expected String" );	
		  }
		  
		  return( val );
        }

        #endregion // Helpers

        #region Private, Methods Implementing Additional Math Functions

        /// <summary>
        /// Calculates the faculty.
        /// </summary>
        /// <remarks>
        /// This method should move out of this class since it has nothing to do with the parser.
        /// it's here because the language math functions do not include faculty calculations.
        /// </remarks>
        /// <param name="val">the value to calcualte the faculty of</param>
        /// <returns>the faculty</returns>
        private double
        fac(double val)
        {

            if (!isInteger(val))
            {
                return Double.NaN;
            }
            else if (val < 0)
            {
                return Double.NaN;
            }
            else if (val <= 1)
            {
                return 1;
            }

            return (val * fac(val - 1));
        }

        /// <summary>
        /// Calculates the semi faculty.
        /// </summary>
        /// <remarks>
        /// This method should move out of this class since it has nothing to do with the parser.
        /// it's here because the language math functions do not include semi faculty calculations.
        /// </remarks>
        /// <param name="val">the value to calcualte the semi faculty of</param>
        /// <returns>the semi faculty</returns>
        private double
        sfac(double val)
        {

            if (!isInteger(val))
            {
                return Double.NaN;
            }
            else if (val < 0)
            {
                return Double.NaN;
            }
            else if (val <= 1)
            {
                return 1;
            }

            return (val * sfac(val - 2));
        }

        /// <summary>
        /// Returns the decimal part of the value
        /// </summary>
        /// <param name="val">the value to calculate the fpart for</param>
        /// <returns>the decimal part of the value</returns>
        private double
        fpart(double val)
        {
            if (val >= 0)
            {
                return (val - Math.Floor(val));
            }
            else
            {
                return (val - Math.Ceiling(val));
            }
        }
        #endregion

        #endregion // All private methods

    } // End class ExpressionParser

} // End namespace info.lundin.math