﻿using GetStoreApp.Models.Controls.History;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.History
{
    public interface IHistoryDBService
    {
        Task AddAsync(HistoryModel history);

        Task<(List<HistoryModel>, bool, bool)> QueryAllAsync(bool timeSortOrder, string typeFilter, string channelFilter);

        Task<List<HistoryModel>> QueryAsync(int value);

        Task<bool> DeleteAsync(List<HistoryModel> selectedHistoryDataList);

        Task<bool> ClearAsync();
    }
}