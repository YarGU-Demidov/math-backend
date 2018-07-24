﻿using System.Linq;
using MathSite.Api.Db.DataSeeding.StaticData;
using MathSite.Api.Entities;
using Microsoft.Extensions.Logging;

namespace MathSite.Api.Db.DataSeeding.Seeders
{
    /// <inheritdoc />
    public class GroupRightsSeeder : AbstractSeeder<GroupsRight>
    {
        /// <inheritdoc />
        public GroupRightsSeeder(ILogger logger, MathSiteDbContext context) : base(logger, context)
        {
        }

        /// <inheritdoc />
        public override string SeedingObjectName { get; } = nameof(GroupsRight);

        /// <inheritdoc />
        protected override void SeedData()
        {
            var adminGroup = GetGroupByAlias(GroupAliases.Admin);
            var usersGroup = GetGroupByAlias(GroupAliases.User);

            var adminAccessRight = GetRightByAlias(RightAliases.AdminAccess);
            var logoutAccessRight = GetRightByAlias(RightAliases.LogoutAccess);
            var panelAccessRight = GetRightByAlias(RightAliases.PanelAccess);
            var setSiteSettingAccessRight = GetRightByAlias(RightAliases.SetSiteSettingsAccess);
            var manageNewsAccessRight = GetRightByAlias(RightAliases.ManageNewsAccess);

            var adminRights = new[]
            {
                CreateGroupRights(true, adminGroup, adminAccessRight),
                CreateGroupRights(true, adminGroup, logoutAccessRight),
                CreateGroupRights(true, adminGroup, panelAccessRight),
                CreateGroupRights(true, adminGroup, setSiteSettingAccessRight),
                CreateGroupRights(true, adminGroup, manageNewsAccessRight)
            };

            var usersRights = new[]
            {
                CreateGroupRights(false, usersGroup, adminAccessRight),
                CreateGroupRights(true, usersGroup, logoutAccessRight),
                CreateGroupRights(true, usersGroup, panelAccessRight),
                CreateGroupRights(false, usersGroup, setSiteSettingAccessRight),
                CreateGroupRights(false, usersGroup, manageNewsAccessRight)
            };

            Context.GroupsRights.AddRange(usersRights);
            Dispose();

            Context.GroupsRights.AddRange(adminRights);
            Dispose();
        }

        private Right GetRightByAlias(string alias)
        {
            return Context.Rights.First(right => right.Alias == alias);
        }

        private Group GetGroupByAlias(string alias)
        {
            return Context.Groups.First(group => group.Alias == alias);
        }

        private static GroupsRight CreateGroupRights(bool allowed, Group group, Right right)
        {
            return new GroupsRight
            {
                Allowed = allowed,
                Group = group,
                Right = right
            };
        }
    }
}