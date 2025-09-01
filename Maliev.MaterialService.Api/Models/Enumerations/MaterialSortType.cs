// <copyright file="MaterialSortType.cs" company="Maliev Company Limited">
// Copyright (c) Maliev Company Limited. All rights reserved.
// </copyright>

namespace Maliev.MaterialService.Api.Models.Enumerations
{
    /// <summary>
    ///   <see cref="MaterialSortType" />.
    /// </summary>
    public enum MaterialSortType
    {
        /// <summary>
        /// The material identifier ascending
        /// </summary>
        MaterialId_Ascending,

        /// <summary>
        /// The material identifier descending
        /// </summary>
        MaterialId_Descending,

        /// <summary>
        /// The material machinability ascending
        /// </summary>
        MaterialMachinability_Ascending,

        /// <summary>
        /// The material machinability descending
        /// </summary>
        MaterialMachinability_Descending,

        /// <summary>
        /// The created date ascending
        /// </summary>
        MaterialCreatedDate_Ascending,

        /// <summary>
        /// The material created date descending
        /// </summary>
        MaterialCreatedDate_Descending,

        /// <summary>
        /// The modified date descending
        /// </summary>
        MaterialModifiedDate_Descending,

        /// <summary>
        /// The material modified date ascending
        /// </summary>
        MaterialModifiedDate_Ascending,

        /// <summary>
        /// The material name ascending
        /// </summary>
        MaterialName_Ascending,

        /// <summary>
        /// The material name descending
        /// </summary>
        MaterialName_Descending,

        /// <summary>
        /// The material group ascending
        /// </summary>
        MaterialGroup_Ascending,

        /// <summary>
        /// The material group descending
        /// </summary>
        MaterialGroup_Descending,

        /// <summary>
        /// The material density ascending
        /// </summary>
        MaterialDensity_Ascending,

        /// <summary>
        /// The material density descending
        /// </summary>
        MaterialDensity_Descending,

        /// <summary>
        /// The thermal conductivity ascending
        /// </summary>
        MaterialThermalConductivity_Ascending,

        /// <summary>
        /// The thermal conductivity descending
        /// </summary>
        MaterialThermalConductivity_Descending,

        /// <summary>
        /// The material number ascending
        /// </summary>
        MaterialNumber_Ascending,

        /// <summary>
        /// The material number descending
        /// </summary>
        MaterialNumber_Descending,
    }
}