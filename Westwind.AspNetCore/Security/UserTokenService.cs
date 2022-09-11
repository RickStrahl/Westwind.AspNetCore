using System;
using Westwind.Utilities;
using Westwind.Utilities.Data;

namespace Westwind.AspNetCode.Security
{

    /// <summary>
    /// Generic SQL Token generator class that can be used to
    /// generate, store and validation user tokens for use in APIs.
    ///
    /// Methods can create tokens, store them and then can be validated
    /// based on existance and timeout status. Service cleans up expired
    /// tokens as part of operations.
    ///
    /// Works with SQL Server but can be customized
    /// </summary>
    public class UserTokenManager
    {
        /// <summary>
        /// Sql Connection String (Sql Server)
        /// </summary>
        public string TokenServerConnectionString { get; set; }

        public string Tablename { get; set; } = "UserTokens";

        public int TokenTimeoutSeconds { get; set; } = 1800;

        public string ErrorMessage { get; set; }

        public UserTokenManager(string connectionString = null)
        {
            TokenServerConnectionString = connectionString;
        }


        /// <summary>
        /// Check to see if a token is valid
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="renewLease">If true updates the Updated property and moves the expiration window out</param>
        /// <returns></returns>
        public bool IsTokenValid(string tokenId, bool renewLease = true)
        {
            var token = GetToken(tokenId, checkForExpiration: true, renewLease: renewLease);

            if (token == null)
                return false;

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="checkForExpiration">if true doesn't retrieve token if it's expired</param>
        /// <param name="renewLease">If true updates the updated property so expiration window moves out</param>
        /// <returns></returns>
        public UserToken GetToken(string tokenId, bool checkForExpiration = true, bool renewLease = true)
        {
            UserToken token;
            using (var data = GetSqlData())
            {
                string sql = $@"select  Top 1 * from [{Tablename}] where Id = @0 Order by Updated Desc";

                token = data.Find<UserToken>(sql, tokenId);
                if (token == null)
                {
                    SetError(data.ErrorMessage);
                    return null;
                }

                if (checkForExpiration && token.Updated.AddSeconds(TokenTimeoutSeconds) < DateTime.UtcNow)
                {
                    SetError("Token has expired.");
                    return null;
                }

                if (renewLease)
                {
                    sql = $@"update [{Tablename}] set updated = GetUtcDate() where Id = @0";
                    data.ExecuteNonQuery(sql, tokenId);
                }
            }

            return token;
        }

        /// <summary>
        /// adds a new token record into the db and returns the new token id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="referenceId"></param>
        /// <returns>A new token Id</returns>
        public string CreateNewToken(string userId, string referenceId = null)
        {
            string tokenId;
            int result;
            using (var data = GetSqlData())
            {
                tokenId = null;
                string sql;
                result = -1;

                var token = data.Find<UserToken>($"select Top 1 * from [{Tablename}] where userId=@0", userId);

                // error - table doesn't exist?
                if (!string.IsNullOrEmpty(data.ErrorMessage))
                {
                    if (!IsUserTokenTable() && CreateUserTokenSqlTable())
                    {
                        return CreateNewToken(userId, referenceId); // retry
                    }
                }

                if (token == null)
                {
                    tokenId = DataUtils.GenerateUniqueId(15);

                    sql = $@"
insert into [{Tablename}]
            (Id,UserId,ReferenceId) Values
            (@0,@1,@2)
";
                    result = data.ExecuteNonQuery(sql, tokenId, userId, referenceId);
                }
                else
                {
                    tokenId = DataUtils.GenerateUniqueId(15);
                    sql = $@"update [{Tablename}] set Id=@0, UserId=@1, ReferenceId=@2, Updated=@3 where Id=@4";
                    result = data.ExecuteNonQuery(sql, tokenId, userId, referenceId ?? token.ReferenceId, DateTime.UtcNow, token.Id);
                }
            }

            if (result == -1)
                return null;

            return tokenId;
        }


        /// <summary>
        /// Deletes a token by its token id
        /// </summary>
        /// <param name="tokenId"></param>
        public bool DeleteToken(string tokenId)
        {
            using (var data = GetSqlData())
            {
                int result = data.ExecuteNonQuery($"delete from [{Tablename}] where id=@0", tokenId);
                if (result == -1)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes a token by its token id
        /// </summary>
        /// <param name="userId">The userid for which do delete tokens</param>
        public bool DeleteTokenForUserId(string userId)
        {
            var data = GetSqlData();
            int result = data.ExecuteNonQuery($"delete from [{Tablename}] where userId=@0", userId);
            if (result == -1)
            {
                SetError(data.ErrorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes a token by its token id
        /// </summary>
        public bool DeleteExpiredTokens()
        {
            var time = DateTime.UtcNow.AddSeconds((TokenTimeoutSeconds * 1.5) * -1);

            var data = GetSqlData();
            int result = data.ExecuteNonQuery($"delete from [{Tablename}] where updated < @0", time);
            if (result == -1)
            {
                SetError(data.ErrorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates the UserToken Sql Server table in the specified connection.
        /// Uses the current 'Tablename' property to determine the table
        ///
        /// Default implementation creates table for SQL Server
        /// </summary>
        /// <returns></returns>
        public virtual bool CreateUserTokenSqlTable()
        {
            using (var data = GetSqlData())
            {
                string sql = $@"
Begin Transaction T1
CREATE TABLE [{Tablename}](
	[Id] [nvarchar](20) NOT NULL,
	[UserId] [nvarchar](100) NULL,
	[ReferenceId] [nvarchar](255) NULL,
	[Updated] [datetime] NOT NULL
) ON [PRIMARY]
ALTER TABLE [{Tablename}] ADD  CONSTRAINT [DF_{Tablename}_Updated]  DEFAULT (getutcdate()) FOR [Updated]
Commit Transaction T1
";

                if (data.ExecuteNonQuery(sql) < 0)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check to see if hte user token table exists
        /// </summary>
        /// <returns></returns>
        public bool IsUserTokenTable()
        {
            using (var data = GetSqlData() )
            {
                data.ThrowExceptions = false;
                object result = data.ExecuteScalar($"select count(*) from  [{Tablename}]");

                if (result == null)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
        }

        #region Helpers

        SqlDataAccess GetSqlData()
        {
            var data = new SqlDataAccess(TokenServerConnectionString) {ThrowExceptions = false};
            return data;
        }

        #endregion

        #region Error Handling

        protected void SetError()
        {
            SetError("CLEAR");
        }

        protected void SetError(string message)
        {
            if (message == null || message == "CLEAR")
            {
                this.ErrorMessage = string.Empty;
                return;
            }
            this.ErrorMessage += message;
        }

        protected void SetError(Exception ex, bool checkInner = false)
        {
            if (ex == null)
                this.ErrorMessage = string.Empty;

            Exception e = ex;
            if (checkInner)
                e = e.GetBaseException();

            ErrorMessage = e.Message;
        }

        #endregion
    }



    public class UserToken
    {
        public UserToken()
        {
            Id = DataUtils.GenerateUniqueId(15);
            Updated = DateTime.UtcNow;
        }

        /// <summary>
        /// This is the token's unique Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Time the token was last updated. Used to determine
        /// when the token expires
        /// </summary>
        public DateTime Updated { get; set; }


        /// <summary>
        /// A user id that maps this token to a given user
        /// id or other user identifier
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// An additional reference string value that can be set
        /// and stored with the key token.
        /// </summary>
        public string ReferenceId { get; set; }
    }

}
