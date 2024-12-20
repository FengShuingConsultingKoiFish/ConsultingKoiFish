﻿using ConsultingKoiFish.BLL.DTOs.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConsultingKoiFish.API
{
	public class BaseAPIController : ControllerBase
	{
		/// <summary>
		/// Errors the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="data">The extend data.</param>
		/// <returns></returns>
		protected ActionResult Error(string message, object data = null)
		{
			return new BadRequestObjectResult(new ResponseApiDTO
			{
				Result = data,
				StatusCode = System.Net.HttpStatusCode.BadRequest,
				Message = message,
			});
		}

		protected ActionResult GetNotFound(string message)
		{
			return new NotFoundObjectResult(new ResponseApiDTO
			{
				IsSuccess = false,
				Message = message,
				StatusCode = System.Net.HttpStatusCode.NotFound
			});
		}

		protected ActionResult GetUnAuthorized(string message)
		{
			return new UnauthorizedObjectResult(new ResponseApiDTO
			{
				IsSuccess = false,
				Message = message,
				StatusCode = System.Net.HttpStatusCode.Unauthorized
			});
		}

		/// <summary>
		/// Gets the data failed.
		/// </summary>
		/// <returns></returns>
		protected ActionResult GetError()
		{
			return Error("Get Data Failed");
		}

		/// <summary>
		/// Gets the data failed.
		/// </summary>
		/// <returns></returns>
		protected ActionResult GetError(string message)
		{
			return Error(message);
		}

		/// <summary>
		/// Saves the data failed.
		/// </summary>
		/// <returns></returns>
		protected ActionResult SaveError(object data = null)
		{
			return Error("Save data failed", data);
		}

		/// <summary>
		/// Models the invalid.
		/// </summary>
		/// <returns></returns>
		protected ActionResult ModelInvalid()
		{
			var errors = ModelState.Where(m => m.Value.Errors.Count > 0)
				.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).First()).ToList();
			return new BadRequestObjectResult(new ResponseApiDTO
			{
				Errors = errors,
				StatusCode = System.Net.HttpStatusCode.BadRequest,
				Message = "Save data failed"
			});
		}

		/// <summary>
		/// Successes request.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		protected ActionResult Success(object data, string message)
		{
			return new OkObjectResult(new ResponseApiDTO
			{
				Result = data,
				StatusCode = System.Net.HttpStatusCode.OK,
				Message = message,
				IsSuccess = true
			});
		}

		/// <summary>
		/// Gets the data successfully.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected ActionResult GetSuccess(object data)
		{
			return Success(data, "Get data success");
		}

		/// <summary>
		/// Saves the data successfully
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected ActionResult SaveSuccess(object data)
		{
			return Success(data, "Save data success");
		}

		/// <summary>
		/// Get the loged in UserNameOrEmail;
		/// </summary>
		protected string UserName => User.FindFirst("Name")?.Value;

		/// <summary>
		/// Get the logged in user email.
		/// </summary>
		protected string UserEmail => User.FindFirst("Email")?.Value;

		/// <summary>
		/// Get the loged in UserId;
		/// </summary>
		protected string UserId
		{
			get
			{
				var id = User.FindFirst("CommentId")?.Value;
				return id;
			}
		}

		/// <summary>
		/// The boolean value that determined whether current user is a Seller
		/// </summary>
		//protected bool IsSeller
		//{
		//	get
		//	{
		//		var isseller = User.FindFirst(Constants.IS_SELLER)?.Value;
		//		bool.TryParse(isseller, out bool isSeller);
		//		return isSeller;
		//	}
		//}

		protected bool IsAdmin
		{
			get
			{
				var isadmin = UserName;
				return isadmin.Equals("Admin");
			}
		}

		//protected bool IsCreator
		//{
		//	get
		//	{
		//		var iscreator = User.FindFirst(Constants.IS_CREATOR)?.Value;
		//		bool.TryParse(iscreator, out bool isCreator);
		//		return isCreator;
		//	}
		//}
	}
}
