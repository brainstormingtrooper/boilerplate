using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using App.Options;
using App.Request;
using Microsoft.AspNetCore.Mvc;
using App.Services;
using static BCrypt.Net.BCrypt;
using UserAvatarResponse = App.Response.v1.UserAvatar;
using UserProfileResponse = App.Response.v1.UserProfile;
using UserPasswordResponse = App.Response.v1.UserPassword;

namespace App.Controllers.v1
{
    [Route("v1/[controller]")]
    public class UserController : Controller
    {
        /// <summary>
        /// Database instance.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="database">Database instance.</param>
        public UserController(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Upload an avatar.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("avatar", Name = "v1.user.avatar")]
        public UserAvatarResponse Avatar([FromForm] UserAvatar request)
        {
            this.ValidateRequest();

            var uploader = new Uploader(request.Avatar);
            var user = this.GetCurrentUser();
            var avatarUploadPath = Path.Combine("/avatar", user.Id.ToString());

            user.Avatar = uploader.UploadTo(avatarUploadPath);

            _database.UpdateUser(user);

            return new UserAvatarResponse
            {
                Message = "OK",
                Data = user
            };
        }

        /// <summary>
        /// Update user profile.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("profile", Name = "v1.user.profile")]
        public UserProfileResponse Profile([FromBody] UserProfile request)
        {
            this.ValidateRequest();

            var user = this.GetCurrentUser()
                           .UpdateFromRequest(request);

            _database.UpdateUser(user);

            return new UserProfileResponse
            {
                Message = "OK",
                Data = user
            };
        }

        /// <summary>
        /// Update user password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("password", Name = "v1.user.password")]
        public UserPasswordResponse Password([FromBody] UserPassword request)
        {
            this.ValidateRequest();

            var user = this.GetCurrentUser()
                           .ChangePassword(request.NewPassword);

            _database.UpdateUser(user);

            return new UserPasswordResponse
            {
                Message = "OK",
                Data = user
            };
        }
    }
}
