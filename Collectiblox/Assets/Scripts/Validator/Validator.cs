using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Collectiblox
{
    public class Validator
    {
        public bool isValidated => validationTargets.Count <= 0;
        List<IValidator> validationTargets;

        public Validator()
        {
            validationTargets = new List<IValidator>();
        }

        public void AddValidationTarget(IValidator target)
        {
            if (!validationTargets.Contains(target))
            {
                target.Validated += Target_Validated;
                validationTargets.Add(target);
            }
        }

        private void Target_Validated(IValidator target)
        {
            validationTargets.Remove(target);
        }
    }
}