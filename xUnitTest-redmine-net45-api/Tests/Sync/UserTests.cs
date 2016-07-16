﻿/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Specialized;
using System.Globalization;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Exceptions;
using Xunit;

namespace xUnitTestredminenet45api
{
	[Trait("Redmine-Net-Api", "Users")]
	[Collection("RedmineCollection")]
	[Order(2)]
	public class UserTests
	{
		private readonly RedmineFixture fixture;

		public UserTests(RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		private const string USER_LOGIN = "testUser";
		private const string USER_FIRST_NAME = "User";
		private const string USER_LAST_NAME = "One";
		private const string USER_EMAIL = "testUser@mail.com";

		private static string CREATED_USER_ID;
		private static string CREATED_USER_WITH_ALL_PROP_ID;

		private static User CreateTestUserWithRequiredPropertiesSet()
		{
			var user = new User()
			{
				Login = USER_LOGIN,
				FirstName = USER_FIRST_NAME,
				LastName = USER_LAST_NAME,
				Email = USER_EMAIL,
			};

			return user;
		}

		[Fact, Order(1)]
		public void Should_Create_User_With_Required_Properties()
		{
			var savedUser = fixture.RedmineManager.CreateObject(CreateTestUserWithRequiredPropertiesSet());

			Assert.NotNull(savedUser);
			Assert.NotEqual(savedUser.Id, 0);

			CREATED_USER_ID = savedUser.Id.ToString();

			Assert.True(savedUser.Login.Equals(USER_LOGIN), "User login is invalid.");
			Assert.True(savedUser.FirstName.Equals(USER_FIRST_NAME), "User first name is invalid.");
			Assert.True(savedUser.LastName.Equals(USER_LAST_NAME), "User last name is invalid.");
			Assert.True(savedUser.Email.Equals(USER_EMAIL), "User email is invalid.");
		}

		[Fact, Order(2)]
		public void Should_Throw_Exception_When_Create_Empty_User()
		{
			Assert.Throws<RedmineException>(() => fixture.RedmineManager.CreateObject(new User()));
		}

		[Fact, Order(3)]
		public void Should_Create_User_With_All_Properties_Set()
		{
			var login = "testUserAllProp";
			var firstName = "firstName";
			var lastName = "lastName";
			var email = "email@a.com";
			var password = "pass123456";
			var mailNotification = "only_assigned";

			var savedUser = fixture.RedmineManager.CreateObject(new User()
			{
				Login = login,
				FirstName = firstName,
				LastName = lastName,
				Email = email,
				Password = password,
				MustChangePassword = true,
			});

			Assert.NotNull(savedUser);
			Assert.NotEqual(savedUser.Id, 0);

			CREATED_USER_WITH_ALL_PROP_ID = savedUser.Id.ToString();

			Assert.True(savedUser.Login.Equals(login), "User login is invalid.");
			Assert.True(savedUser.FirstName.Equals(firstName), "User first name is invalid.");
			Assert.True(savedUser.LastName.Equals(lastName), "User last name is invalid.");
			Assert.True(savedUser.Email.Equals(email), "User email is invalid.");
		}

		[Fact, Order(4)]
		public void Should_Get_Created_User_With_Required_Fields()
		{
			var user = fixture.RedmineManager.GetObject<User>(CREATED_USER_ID, null);

			Assert.NotNull(user);
			Assert.IsType<User>(user);
			Assert.True(user.Login.Equals(USER_LOGIN), "User login is invalid.");
			Assert.True(user.FirstName.Equals(USER_FIRST_NAME), "User first name is invalid.");
			Assert.True(user.LastName.Equals(USER_LAST_NAME), "User last name is invalid.");
			Assert.True(user.Email.Equals(USER_EMAIL), "User email is invalid.");
		}

		[Fact, Order(5)]
		public void Should_Update_User()
		{
			const string UPDATED_USER_FIRST_NAME = "UpdatedFirstName";
			const string UPDATED_USER_LAST_NAME = "UpdatedLastName";
			const string UPDATED_USER_EMAIL = "updatedEmail@mail.com";

			var user = fixture.RedmineManager.GetObject<User>(CREATED_USER_ID, null);
			user.FirstName = UPDATED_USER_FIRST_NAME;
			user.LastName = UPDATED_USER_LAST_NAME;
			user.Email = UPDATED_USER_EMAIL;

			var exception =
				(RedmineException)
				Record.Exception(() => fixture.RedmineManager.UpdateObject(CREATED_USER_ID, user));
			Assert.Null(exception);

			var updatedUser = fixture.RedmineManager.GetObject<User>(CREATED_USER_ID, null);

			Assert.True(updatedUser.FirstName.Equals(UPDATED_USER_FIRST_NAME), "User first name was not updated.");
			Assert.True(updatedUser.LastName.Equals(UPDATED_USER_LAST_NAME), "User last name was not updated.");
			Assert.True(updatedUser.Email.Equals(UPDATED_USER_EMAIL), "User email was not updated.");
		}

		[Fact, Order(6)]
		public void Should_Not_Update_User_With_Invalid_Properties()
		{
			var user = fixture.RedmineManager.GetObject<User>(CREATED_USER_ID, null);
			user.FirstName = "";

			Assert.Throws<RedmineException>(() => fixture.RedmineManager.UpdateObject(CREATED_USER_ID, user));
		}

		[Fact, Order(7)]
		public void Should_Delete_User()
		{
			var exception =
				(RedmineException)
				Record.Exception(() => fixture.RedmineManager.DeleteObject<User>(CREATED_USER_ID, null));
			Assert.Null(exception);
			Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<User>(CREATED_USER_ID, null));

		}

		[Fact, Order(8)]
		public void Should_Delete_User_Created_With_All_Properties_Set()
		{
			var exception =
				(RedmineException)
				Record.Exception(() => fixture.RedmineManager.DeleteObject<User>(CREATED_USER_WITH_ALL_PROP_ID, null));
			Assert.Null(exception);
			Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<User>(CREATED_USER_WITH_ALL_PROP_ID, null));

		}

		[Fact, Order(9)]
		public void Should_Get_Current_User()
		{
			User currentUser = fixture.RedmineManager.GetCurrentUser();

			Assert.NotNull(currentUser);
			Assert.Equal(currentUser.ApiKey, Helper.ApiKey);
		}

		[Fact, Order(10)]
		public void Should_Get_X_Users_From_Offset_Y()
		{
			var result = fixture.RedmineManager.GetPaginatedObjects<User>(new NameValueCollection()
			{
				{RedmineKeys.INCLUDE, RedmineKeys.GROUPS + "," + RedmineKeys.MEMBERSHIPS},
				{RedmineKeys.LIMIT, "2"},
				{RedmineKeys.OFFSET, "1"}
			});

			Assert.NotNull(result);
			Assert.All(result.Objects, u => Assert.IsType<User>(u));
		}

		[Fact, Order(11)]
		public void Should_Get_Users_By_State()
		{
			var users = fixture.RedmineManager.GetObjects<User>(new NameValueCollection()
			{
				{RedmineKeys.STATUS, ((int) UserStatus.STATUS_ACTIVE).ToString(CultureInfo.InvariantCulture)}
			});

			Assert.NotNull(users);
			Assert.All(users, u => Assert.IsType<User>(u));
		}
	}
}