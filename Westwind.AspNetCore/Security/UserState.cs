
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Westwind.Utilities;

namespace Westwind.Web
{
    /// <summary>
    /// User information container that can easily 'serialize'
    /// to a string and back. Meant to hold basic logon information
    /// to avoid trips to the database for common information required
    /// by an app to validate and display user info.
    ///
    /// The **UserId** (or Int/Guid) value should always be set with
    /// a value to indicate that this object is not empty.
    /// 
    /// I use this class a lot to attach to serialize as a singl
    /// User Claim in User Claims, or Forms Authentication tickets
    /// </summary>
    [DebuggerDisplay("{Name ?? \"Empty\"}")]
    public class UserState
    {
        public UserState()
        {        
            Name = string.Empty;
            Email = string.Empty;
            UserId = string.Empty;
            UserIdInt = -1;
            IsAdmin = false;
            SecurityToken = string.Empty;
        }

        /// <summary>
        /// The display name for the userId
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user's email address or login acount
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's user Id as a string
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The users admin status
        /// </summary>
        public bool IsAdmin { get; set; }
        
        
        /// <summary>
        /// Returns the User Id as an int if convertiable
        /// </summary>
        public int UserIdInt { get; set; }

        /// <summary>
        /// Returns the User Id as an int if convertiable
        /// </summary>
        public Guid? UserIdGuid { get; set; }

        /// <summary>
        /// A unique id created for this entry that can be used to
        /// identify the user outside of the UserState context
        /// </summary>
        public string SecurityToken { 
            get 
            {
                if (string.IsNullOrEmpty(_SecurityToken))
                    _SecurityToken = StringUtils.NewStringId();

                return _SecurityToken;
            }
            set
            {
                _SecurityToken = value;
            }
        }
        private string _SecurityToken = null;

        

        /// <summary>
        /// Exports a short string list of Id, Email, Name separated by |
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StringSerializer.SerializeObject(this);
        }

        /// <summary>
        /// Imports Id, Email and Name from a | separated string
        /// </summary>
        /// <param name="itemString"></param>
        public bool FromString(string itemString)
        {
            if (string.IsNullOrEmpty(itemString))
                return false;

            var state = CreateFromString(itemString);
            if (state == null)
                return false;

            // copy the properties
            DataUtils.CopyObjectData(state, this);

            return true;
        }


        /// <summary>
        /// Creates an instance of a userstate object from serialized
        /// data.
        /// 
        /// IsEmpty() will return true if data was not loaded. A 
        /// UserData object is always returned.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static UserState CreateFromString(string userData)
        {
            return CreateFromString<UserState>(userData);            
        }

        /// <summary>
        /// Creates an instance of a userstate object from serialized
        /// data.
        /// 
        /// IsEmpty() will return true if data was not loaded. A 
        /// UserData object is always returned.
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="userStateType"></param>
        /// <returns></returns>
        public static UserState CreateFromString(string userData, Type userStateType)
        {
            if (string.IsNullOrEmpty(userData))
                return null;

            UserState result = null;
            try
            {
                object res = StringSerializer.DeserializeObject(userData, userStateType);
                result = res as UserState;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UserState Deserialization failed: " + ex.Message);
                return Activator.CreateInstance(userStateType) as UserState;
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of a userstate object from serialized
        /// data.
        /// 
        /// IsEmpty() will return true if data was not loaded. A 
        /// UserData object is always returned.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static T CreateFromString<T>(string userData)
            where T :  class, new()
        {
            if (string.IsNullOrEmpty(userData))
                return null;
            
            T result = null;
            try
            {
                object res = StringSerializer.DeserializeObject(userData, typeof(T));
                result = res as T;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("UserState Deserialization failed: " + ex.Message);
                return new T();
            }

            return result;
        }

       


        /// <summary>
        /// Creates a UserState object from authentication information in the 
        /// Forms Authentication ticket.
        /// 
        /// IsEmpty() will return false if no data was loaded but
        /// a Userdata object is always returned
        /// </summary>
        /// <returns></returns>
        public static UserState CreateFromFormsAuthTicket(HttpContext context)
        {            
            var identity = context.User.Identity as ClaimsIdentity;
            if (identity == null)
                return new UserState();

            
            var claim = identity.FindFirst("UserState");
            if (claim == null)
                return new UserState();

            return CreateFromString(claim.Value) as UserState;
        }
        
        /// <summary>
        /// Creates a UserState object from authentication information in the 
        /// Forms Authentication ticket.
        /// 
        /// IsEmpty() will return false if no data was loaded but
        /// a Userdata object is always returned
        /// </summary>
        /// <returns></returns>
        public static T CreateFromUserClaims<T>(HttpContext context)
            where T : UserState, new()
        {
            return CreateFromUserClaims(context, typeof(T)) as T;            
        }

        /// <summary>
        /// Creates a UserState object from authentication information in the 
        /// Forms Authentication ticket.
        /// 
        /// IsEmpty() will return false if no data was loaded but
        /// a Userdata object is always returned
        /// </summary>
        /// <param name="context">Http context that holds Identity and User Claims</param>
        /// <returns></returns>
        public static UserState CreateFromUserClaims(HttpContext context)
        {
            return CreateFromUserClaims<UserState>(context);
        }

        /// <summary>
        /// Creates a UserState object from authentication information in the 
        /// Forms Authentication ticket.
        /// 
        /// IsEmpty() will return false if no data was loaded but
        /// a Userdata object is always returned
        /// </summary>
        /// <param name="context">Http context that holds Identity and User Claims</param>
        /// <param name="userStateType">UserState type to deserialize into</param>
        /// <returns></returns>
        public static UserState CreateFromUserClaims(HttpContext context, Type userStateType)
        {
            var identity = context.User?.Identity as ClaimsIdentity;
            if (identity == null)
                return Activator.CreateInstance(userStateType) as UserState;

            var claim = identity.FindFirst("UserState");
            if (claim == null)
                return Activator.CreateInstance(userStateType) as UserState;

            var state = CreateFromString(claim.Value,userStateType);
            
            return state;
        }

        
        /// <summary>
        /// Determines whether UserState instance
        /// holds user information - specifically
        /// whether one of the UserID values is set.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(UserId) &&
                   UserIdInt < 1
                   && (UserIdGuid==null || UserIdGuid == Guid.Empty);
        }
    }
}
