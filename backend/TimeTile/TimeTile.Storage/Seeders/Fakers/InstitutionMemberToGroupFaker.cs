using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class InstitutionMemberToGroupFaker : BaseFaker<InstitutionMemberToGroup>
    {
        private readonly List<(int InstitutionMemberId, int GroupId)> _possiblePairs = new();

        private int _actualIndex = 0;

        public InstitutionMemberToGroupFaker(List<InstitutionMember> institutionMembers, List<Group> groups)
        {
            FindAllPossibleCombinations(institutionMembers, groups);

            _possiblePairs = _possiblePairs.OrderBy(_ => Guid.NewGuid()).ToList();

            _faker
                .Rules((faker, entity) =>
                {
                    (int, int) element = _possiblePairs.ElementAt(_actualIndex);

                    entity.InstitutionMemberId = element.Item1;
                    entity.GroupId = element.Item2;

                    _actualIndex++;
                });
        }

        private void FindAllPossibleCombinations(List<InstitutionMember> institutionMembers, List<Group> groups)
        {
            var commonInstitutionsIds = FindCommonInstitutionsIds(institutionMembers, groups);

            if (commonInstitutionsIds.Count() == 0)
                throw new InvalidOperationException("There are no associations between institution members and groups with the same institution.");


            var suitableInstitutionMembers = institutionMembers
                .Where(m => commonInstitutionsIds.Contains(m.InstitutionId!.Value));

            if (suitableInstitutionMembers.Count() == 0)
                throw new InvalidOperationException("There are no associations between institution members and groups.");


            var suitableGroups = groups
                .Where(g => commonInstitutionsIds.Contains(g.InstitutionId));


            AddAllPossibleCombinations(suitableInstitutionMembers, suitableGroups);
        }

        private IEnumerable<int> FindCommonInstitutionsIds(List<InstitutionMember> institutionMembers, List<Group> groups)
        {
            HashSet<int> institutionMembersInstitutionsIds = new();
            foreach (var member in institutionMembers)
                institutionMembersInstitutionsIds.Add(member.InstitutionId!.Value);

            HashSet<int> groupsInstitutionsIds = new();
            foreach (var group in groups)
                groupsInstitutionsIds.Add(group.InstitutionId);

            return institutionMembersInstitutionsIds.Intersect(groupsInstitutionsIds);
        }

        private void AddAllPossibleCombinations(IEnumerable<InstitutionMember> suitableInstitutionMembers, IEnumerable<Group> suitableGroups)
        {
            foreach (var institutionMember in suitableInstitutionMembers)
            {
                foreach (var group in suitableGroups)
                {
                    if (institutionMember.InstitutionId == group.InstitutionId)
                    {
                        _possiblePairs.Add((institutionMember.Id, group.Id));
                    }
                }
            }
        }

        public override List<InstitutionMemberToGroup> Generate(int count)
        {
            int rest = _possiblePairs.Count - _actualIndex;

            if (count > rest)
            {
                return base.Generate(rest);
            }    

            return base.Generate(count);
        }
    }
}
