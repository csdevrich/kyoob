﻿using System;
using System.Collections.Generic;

namespace Kyoob.Debug
{
    /// <summary>
    /// The callback delegate type for commands.
    /// </summary>
    /// <param name="param">The collection of passed in parameters.</param>
    public delegate void CommandCallback( string[] param );

    /// <summary>
    /// The class that handles commands.
    /// </summary>
    public sealed class CommandHandler
    {
        private Dictionary<string, Dictionary<string, CommandCallback>> _callbacks;

        /// <summary>
        /// Creates a new command handler.
        /// </summary>
        public CommandHandler()
        {
            _callbacks = new Dictionary<string, Dictionary<string, CommandCallback>>();

            // add listcmd command
            AddCallback( "", "listcmd", ( string[] param ) =>
            {
                Terminal.WriteInfo( "Terminal commands:" );
                foreach ( string obj in _callbacks.Keys )
                {
                    foreach ( string func in _callbacks[ obj ].Keys )
                    {
                        Terminal.WriteInfo( "    {0}{1}", ( obj == "" ) ? "" : obj + ".", func );
                    }
                }
            } );
        }

        /// <summary>
        /// Executes a function.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="func">The function.</param>
        /// <param name="param">The parameter.</param>
        public void Execute( string obj, string func, string[] param )
        {
            if ( _callbacks.ContainsKey( obj ) )
            {
                var functions = _callbacks[ obj ];
                if ( functions.ContainsKey( func ) )
                {
                    // get the callback
                    CommandCallback callback = functions[ func ];
                    if ( callback != null )
                    {
                        // we'll handle exceptions for the callback
                        try
                        {
                            callback( param );
                        }
                        catch ( Exception ex )
                        {
                            Terminal.WriteError( ex.Message );
                        }
                    }
                    else
                    {
                        Terminal.WriteError( "'{0}{1}' is registered, but unbound", ( obj == "" ) ? "" : obj + ".", func );
                    } // if callback not null
                }
                else
                {
                    Terminal.WriteError( "Could not find function '{0}{1}'", ( obj == "" ) ? "" : obj + ".", func );
                } // if functions.contains func
            }
            else
            {
                Terminal.WriteError( "Could not find object '{0}'", obj );
            } // if callbacks.contains obj
        }

        /// <summary>
        /// Checks to see if the command handler has an object registered.
        /// </summary>
        /// <param name="obj">The object name.</param>
        public bool HasObject( string obj )
        {
            obj = obj.ToLower();

            return _callbacks.ContainsKey( obj );
        }

        /// <summary>
        /// Checks to see if the command handler has a function registered.
        /// </summary>
        /// <param name="obj">The object name.</param>
        /// <param name="func">The function name.</param>
        /// <returns></returns>
        public bool HasFunction( string obj, string func )
        {
            obj = obj.ToLower();
            func = func.ToLower();

            if ( !HasObject( obj ) )
            {
                return false;
            }

            return _callbacks[ obj ].ContainsKey( func );
        }

        /// <summary>
        /// Adds a callback function to the command handler.
        /// </summary>
        /// <param name="obj">The object name.</param>
        /// <param name="func">The function name.</param>
        /// <param name="callback">The callback.</param>
        public void AddCallback( string obj, string func, CommandCallback callback )
        {
            obj = obj.ToLower();
            func = func.ToLower();

            if ( _callbacks.ContainsKey( obj ) )
            {
                var functions = _callbacks[ obj ];
                if ( functions.ContainsKey( func ) )
                {
                    string message = string.Format(
                        "The function '{0}{1}' already exists.",
                        ( obj == "" ) ? "" : obj + ".", func
                    );
                    throw new ArgumentException( message );
                }
                else
                {
                    functions.Add( func, callback );
                }
            }
            else
            {
                var functions = new Dictionary<string, CommandCallback>();
                functions.Add( func, callback );
                _callbacks.Add( obj, functions );
            }
        }
        
        /// <summary>
        /// Attempts to parse a line of command text.
        /// </summary>
        /// <param name="command">The command.</param>
        public void ParseCommand( string command )
        {
            // don't even bother with the command if there is none
            if ( string.IsNullOrEmpty( command ) )
            {
                return;
            }
            command = command.ToLower();

            try
            {
                // try to get the object
                int index = command.IndexOf( '.' );
                string obj = "";
                if ( index != -1 )
                {
                    obj = command.Substring( 0, index );
                }

                // try to get the function
                int start = index + 1;
                index = command.IndexOf( ' ' );
                string func = "";
                if ( index == -1 )
                {
                    func = command.Substring( start, command.Length - start );
                }
                else
                {
                    func = command.Substring( start, index - start );
                }

                // get parameters to pass in then execute the callback
                start = index + 1;
                string[] param = command.Substring( start, command.Length - start ).Split( ' ' );

                // now attempt to execute the command
                Execute( obj, func, param );
            }
            catch ( Exception )
            {
                Terminal.WriteError( "Failed to parse command" );
            }
        }
    }
}