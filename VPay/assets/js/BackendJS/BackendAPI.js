
var BackendAPI = function (BID, APIUrl) {

    this.updateBID = function  (cb) {
        var url = APIUrl + "/UpdateBID";        

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };    
    
    this.updateAllCompanyKey = function (cb) {
        var url = APIUrl + "/UpdateAllCompanyKey";
        var postData = {
            BID:BID
        }
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalReport8 = function (companyID, startDate, endDate, withdrawSerial, orderID, status, bankCardName, cb) {
        var url = APIUrl + "/GetWithdrawalReport8";
        var postData;

        postData = {
            BID:BID,
            WithdrawSerial: withdrawSerial,
            OrderID: orderID,
            Startdate: startDate,
            Enddate: endDate,
            Status: status,
            BankCardName: bankCardName,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProviderPointHistory = function (startDate, endDate, providerCode, operatorType, cb) {
        var url = APIUrl + "/GetProviderPointHistory";
        var postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            ProviderCode: providerCode,
            OperatorType: operatorType
        };
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getGPayRelationResult = function (forCompanyID, serviceType, currencyType, cb) {
        var url = APIUrl + "/GetGPayRelationResult";
        var postData = {
            BID: BID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            CompanyID: forCompanyID
        };
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyAllServiceDetailData = function (companyid,cb) {
        var url = APIUrl + "/GetCompanyAllServiceDetailData";
        var postData = {
            BID: BID,
            CompanyID: companyid
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getAllProviderTotalResult = function (cb) {
        var url = APIUrl + "/GetAllProviderTotalResult";

        callServiceByPost(url,BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderListResult = function (cb) {
        var url = APIUrl + "/GetProviderListResult";

        callServiceByPost(url,BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderListResult = function (cb) {
        var url = APIUrl + "/GetProviderListResult";

        callServiceByPost(url,BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.GetCompanyServicePointLogResult = function (postdata, cb) {

        var url = APIUrl + "/GetCompanyServicePointLogResult";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.GetCompanyServicePointLogResultByCompany = function (postdata, cb) {

        var url = APIUrl + "/GetCompanyServicePointLogResultByCompany";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.GetAgentCompanyServicePointLogResultByCompany = function (postdata, cb) {

        var url = APIUrl + "/GetAgentCompanyServicePointLogResultByCompany";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getManualPointLogResult = function (postdata, cb) {

        var url = APIUrl + "/GetManualPointLogResult";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeProviderServiceState = function (ProviderCode, ServiceType, CurrencyType, cb) {
        var url = APIUrl + "/ChangeProviderServiceState";
        var postData;

        postData = {
            BID:BID,
            ProviderCode: ProviderCode,
            ServiceType: ServiceType,
            CurrencyType: CurrencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeProviderCodeState = function (ProviderCode, cb) {
        var url = APIUrl + "/ChangeProviderCodeState";
        var postData;

        postData = {
            BID:BID,
            ProviderCode: ProviderCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeProviderAPIType = function (ProviderCode, setAPIType, cb) {
        var url = APIUrl + "/ChangeProviderAPIType";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: ProviderCode,
            setAPIType: setAPIType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.userLoginByGoogle = function (loginAccount, password, userKey, cb) {
        var url = APIUrl + "/LoginByGoogle";
        var postData;

        postData = {
            BID: BID,
            LoginAccount: loginAccount,
            Password: password,
            UserKey: userKey
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.userLogout = function (cb) {
        var url = APIUrl + "/Logout";

        callServiceByPost(url,BID, function (success, text) {
            cb(true);
        });
    };

    this.searchIPCounty = function (ip, cb) {
        var url = APIUrl + "/SearchIPCounty";
        var postData = {
            BID:BID,
            IP: ip
        }
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#region  CompanyPointHistory
    this.getCompanyPointHistoryTableResult = function (postdata, cb) {

        var url = APIUrl + "/GetCompanyPointHistoryTableResult";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region BankCode
    this.getBankCodeTableResult = function (cb) {
        var url = APIUrl + "/GetBankCodeTableResult";
        var postData;

        postData = {
            BID:BID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertBankCode = function (bankCode, bankName, ETHContractNumber, bankState, bankType, cb) {
        var url = APIUrl + "/InsertBankCode";
        var postData;

        postData = {
            BID:BID,
            BankCode: bankCode,
            BankName: bankName,
            ETHContractNumber: ETHContractNumber,
            BankState: bankState,
            BankType: bankType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateBankCode = function (bankCode, bankName, ETHContractNumber, bankState, bankType, cb) {
        var url = APIUrl + "/UpdateBankCode";
        var postData;

        postData = {
            BID: BID,
            BankCode: bankCode,
            BankName: bankName,
            ETHContractNumber: ETHContractNumber,
            BankState: bankState,
            BankType: bankType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.disableBankCode = function (bankCode, cb) {
        var url = APIUrl + "/DisableBankCode";
        var postData;

        postData = {
            BID: BID,
            BankCode: bankCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);
                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };
    //#endregion 

    //#region BankCard
    this.getBankCardTableResult = function (companyID, cb) {
        var url = APIUrl + "/GetBankCardTableResult";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertBankCard = function (bankCard, bankCode, bankCardName, companyid, ownProvince, ownCity, bankBranchName, cb) {
        var url = APIUrl + "/InsertBankCard";
        var postData;

        postData = {
            BID: BID,
            BankCard: bankCard,
            BankCode: bankCode,
            BankCardName: bankCardName,
            forCompanyID: companyid,
            OwnProvince: ownProvince,
            OwnCity: ownCity,
            BankBranchName: bankBranchName
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateBankCard = function (bankCard, bankCardName, bankCode, ownProvince, ownCity, bankBranchName, cb) {
        var url = APIUrl + "/UpdateBankCard";
        var postData;

        postData = {
            BID: BID,
            BankCardName: bankCardName,
            BankCode: bankCode,
            BankCard: bankCard,
            OwnProvince: ownProvince,
            OwnCity: ownCity,
            BankBranchName: bankBranchName
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.deleteBankCard = function (bankCard, cb) {
        var url = APIUrl + "/DeleteBankCard";
        var postData;

        postData = {
            BID: BID,
            BankCard: bankCard
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    //#region 会员谷歌验证
    this.getGoogleQrCodeByAdmin = function (loginAccount, cb) {

        var url = APIUrl + "/GetGoogleQrCodeByAdmin";
        var postData = {
            BID: BID,
            LoginAccount: loginAccount
        };
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.setGoogleQrCodeByAdmin = function (data, cb) {
        var url = APIUrl + "/SetGoogleQrCodeByAdmin";
        var postData;

        postData = data;
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.unsetGoogleQrCodeByAdmin = function (data, cb) {
        var url = APIUrl + "/UnsetGoogleQrCodeByAdmin";
        var postData;

        postData = data;
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    //#region 提現
    this.getCompanyKey = function (companyid, password, cb) {
        var url = APIUrl + "/GetCompanyKey";
        var postData;

        postData = {
            CompanyID: companyid,
            Password: password
        };
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.unlockCompanyGoogleKey = function (companyid, password, cb) {
        var url = APIUrl + "/RemoveGoogleQrCode";
        var postData;

        postData = {
            CompanyID: companyid,
            Password: password
        };
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getGoogleQrCode = function (cb) {
        var url = APIUrl + "/GetGoogleQrCode";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.setGoogleQrCode = function (data, cb) {
        var url = APIUrl + "/SetGoogleQrCode";
        var postData;

        postData = data;
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.unsetGoogleQrCode = function (data, cb) {
        var url = APIUrl + "/UnsetGoogleQrCode";
        var postData;

        postData = data
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.checkGoogleKey = function (userKey, cb) {

        var url = APIUrl + "/CheckGoogleKey";
        var postData = {
            BID: BID,
            UserKey: userKey
        }
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.removeAllWithdrawal = function (data, cb) {
        var url = APIUrl + "/RemoveAllWithdrawal";
        var postData;

        postData = { WithdrawIDs: data };
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.removeWithdrawal = function (withdrawID, cb) {
        var url = APIUrl + "/RemoveWithdrawal";

        var postData = {
            BID: BID,
            WithdrawID: withdrawID
        }
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.WithdrawalRecord = function (withdrawalSerial, cb) {

        var url = APIUrl + "/WithdrawalRecord";
        var postData = {
            BID: BID,
            WithdrawalSerial: withdrawalSerial
        }
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.tmpWithdrawalUpdate = function (data, cb) {
        var url = APIUrl + "/TmpWithdrawalUpdate";
        var postData;

        postData = {
            WithdrawalData: data
        };
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.withdrawalUpdate = function (data, cb) {
        var url = APIUrl + "/WithdrawalUpdate";
        var postData;

        postData = {
            WithdrawalData: data
        };
        postData["BID"] = BID;
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProviderOrderCount = function (cb) {
        var url = APIUrl + "/GetProviderOrderCount";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalCount = function (cb) {
        var url = APIUrl + "/GetWithdrawalCount";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getRiskControlByPaymentSuccessCount = function (cb) {
        var url = APIUrl + "/GetRiskControlByPaymentSuccessCount";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getRiskControlByWithdrawCount = function (cb) {
        var url = APIUrl + "/GetRiskControlByWithdrawCount";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getRiskControlPayment = function (cb) {
        var url = APIUrl + "/GetRiskControlPayment";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getRiskControlWithdrawal = function (cb) {
        var url = APIUrl + "/GetRiskControlWithdrawal";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.adjustProviderPoint = function (providercode, amount, withdrawSerial, cb) {
        var url = APIUrl + "/AdjustProviderPoint";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providercode,
            Amount: amount,
            WithdrawSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.tmpWithdrawalCreate = function (data, cb) {
        var url = APIUrl + "/TmpWithdrawalCreate";
        var postData;

        postData = {
            BID: BID,
            WithdrawalData: data
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.WithdrawalCreate = function (data,userKey, cb) {
        var url = APIUrl + "/WithdrawalCreate";
        var postData;

        postData = {
            BID: BID,
            WithdrawalData: data,
            UserKey: userKey
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalTableResultByCompanyID = function (cb) {
        var url = APIUrl + "/getWithdrawalTableResultByCompanyID";
        var postData = null;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    this.getProxyProviderData = function (cb) {

        var url = APIUrl + "/GetProxyProviderData";

        callServiceByPost(url,BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.reSendWithdrawal = function (withdrawSerial, isReSendWithdraw, cb) {

        var url = APIUrl + "/ReSendWithdrawal";

        var postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            isReSendWithdraw: isReSendWithdraw
        }
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getWithdrawalReport = function (startDate, endDate, withdrawSerial, orderID, status, bankCardName,  cb) {
        var url = APIUrl + "/GetWithdrawalReport";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            OrderID: orderID,
            Startdate: startDate,
            Enddate: endDate,
            Status: status,
            BankCardName: bankCardName
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalTableResult = function (withdrawSerial, companyid, startdate, enddate, status, cb) {
        var url = APIUrl + "/GetWithdrawalTableResult";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            CompanyID: companyid,
            Startdate: startdate,
            Enddate: enddate,
            Status: status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalResultByWithdrawSerial = function (withdrawSerial, cb) {
        var url = APIUrl + "/GetWithdrawalResultByWithdrawSerial";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.queryAPIWithdrawal = function (withdrawSerial, cb) {
        var url = APIUrl + "/QueryAPIWithdrawal";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalBankSequence = function (withdrawSerial, bankDescription, status, cb) {
        var url = APIUrl + "/UpdateWithdrawalBankSequence";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            BankDescription: bankDescription,
            Status: status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateProxyProviderPoint = function (amount, groupID, providerCode, description, cb) {
        var url = APIUrl + "/UpdateProxyProviderPoint";
        var postData;

        postData = {
            BID: BID,
            Amount: amount,
            GroupID: groupID,
            ProviderCode: providerCode,
            Description: description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateProxyProviderPoint2 = function (orderID, cb) {
        var url = APIUrl + "/UpdateProxyProviderPoint2";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: orderID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.confirmManualProviderPayment = function (paymentSerial, status, bankDescription,userKey, cb) {
        var url = APIUrl + "/ConfirmManualProviderPayment";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: paymentSerial,
            ProcessStatus: status,
            PatchDescription: bankDescription,
            UserKey: userKey
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.confirmManualPayment = function (paymentSerial, status, providerCode, serviceType, providerGroup, cb) {
        var url = APIUrl + "/ConfirmManualPayment";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: paymentSerial,
            ProcessStatus: status,
            ProviderCode: providerCode,
            ServiceType: serviceType,
            GroupID: providerGroup
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.confirmManualPaymentForRunPay = function (paymentSerial, status, providerCode, serviceType, providerGroup, cb) {
        var url = APIUrl + "/ConfirmManualPaymentForRunPay";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: paymentSerial,
            ProcessStatus: status,
            ProviderCode: providerCode,
            ServiceType: serviceType,
            GroupID: providerGroup
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.confirmManualWithdrawal = function (withdrawSerial, status, cb) {
        var url = APIUrl + "/ConfirmManualWithdrawal";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            Status: status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.confirmManualWithdrawalForProxyProivder = function (withdrawSerial, status, bankDescription,userKey, cb) {
        var url = APIUrl + "/ConfirmManualWithdrawalForProxyProivder";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            Status: status,
            BankDescription: bankDescription,
            UserKey: userKey
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.confirmAutoWithdrawal = function (withdrawSerial, cb) {
        var url = APIUrl + "/ConfirmAutoWithdrawal";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalRejectDescription = function (withdrawSerial, rejectDescription, status, cb) {
        var url = APIUrl + "/UpdateWithdrawalRejectDescription";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            RejectDescription: rejectDescription,
            Status: status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getConfigSetting = function (settingKey, cb) {
        var url = APIUrl + "/GetConfigSetting";
        var postData;

        postData = {
            BID: BID,
            SettingKey: settingKey
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawOption = function (withdrawOption, cb) {
        var url = APIUrl + "/UpdateWithdrawOption";
        var postData;

        postData = {
            BID: BID,
            SettingValue: withdrawOption
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalResultByWithdrawSerial = function (withdrawSerial, status, providerCode, withdrawType, serviceType, cb) {
        var url = APIUrl + "/UpdateWithdrawalResultByWithdrawSerial";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            Status: status,
            ProviderCode: providerCode,
            WithdrawType: withdrawType,
            ServiceType: serviceType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalResultByWithdrawSerialForAdjustProfit = function (withdrawSerial, status, providerCode, withdrawType, serviceType, groupID, cb) {
        var url = APIUrl + "/UpdateWithdrawalResultByWithdrawSerialForAdjustProfit";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            Status: status,
            ProviderCode: providerCode,
            WithdrawType: withdrawType,
            ServiceType: serviceType,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    this.updateWithdrawalResultByWithdrawSerialCheck = function (userKey, withdrawSerial, status, cb) {
        var url = APIUrl + "/UpdateWithdrawalResultByWithdrawSerialCheck";
        var postData;

        postData = {
            BID: BID,
            UserKey: userKey,
            WithdrawSerial: withdrawSerial,
            Status: status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalResultByWithdrawSerialDoubleCheck = function (userKey, withdrawSerial, cb) {
        var url = APIUrl + "/UpdateWithdrawalResultByWithdrawSerialDoubleCheck";
        var postData;

        postData = {
            BID: BID,
            UserKey: userKey,
            WithdrawSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalAdminTableResult2 = function (withdrawSerial, companyid, startdate, enddate, status, providerCode, groupID, cb) {
        var url = APIUrl + "/GetWithdrawalAdminTableResult2";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            CompanyID: companyid,
            Startdate: startdate,
            Enddate: enddate,
            Status: status,
            ProviderCode: providerCode,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalAdminTableResult = function (withdrawSerial, companyid, startdate, enddate, status, providerCode, groupID, cb) {
        var url = APIUrl + "/GetWithdrawalAdminTableResult";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            CompanyID: companyid,
            Startdate: startdate,
            Enddate: enddate,
            Status: status,
            ProviderCode: providerCode,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalAdminTableResultForProvider = function (withdrawSerial, startdate, enddate, minAmount, maxAmount, bankDescription, cb) {
        var url = APIUrl + "/GetWithdrawalAdminTableResultForProvider";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            Startdate: startdate,
            Enddate: enddate,
            MinAmount: minAmount,
            MaxAmount: maxAmount,
            BankDescription: bankDescription
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    this.onlySearchWithdrawalForProvider = function (withdrawSerial, startdate, enddate, minAmount, maxAmount, bankDescription, groupID,cb) {
        var url = APIUrl + "/OnlySearchWithdrawalForProvider";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            Startdate: startdate,
            Enddate: enddate,
            MinAmount: minAmount,
            MaxAmount: maxAmount,
            BankDescription: bankDescription,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    this.getWithdrawalTableResultByLstWithdrawID = function (withdrawID, cb) {
        var url = APIUrl + "/GetWithdrawalTableResultByLstWithdrawID";
        var postData;

        postData = {
            BID: BID,
            WithdrawIDs: withdrawID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalTableResultByLstStatus = function (status, cb) {
        var url = APIUrl + "/GetWithdrawalTableResultByLstStatus";
        var postData;

        postData = {
            BID: BID,
            LstStatus: status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    this.getProviderWithdrawalTableResultByStatus = function (cb) {
        var url = APIUrl + "/GetProviderWithdrawalTableResultByStatus";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.onlySearchProviderWithdrawalTableResultByStatus = function (cb) {
        var url = APIUrl + "/OnlySearchProviderWithdrawalTableResultByStatus";
        var postData;


        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getWithdrawalAdminTableResultByCashier = function (withdrawSerial, companyid, startdate, enddate, status, cb) {
        var url = APIUrl + "/GetetWithdrawalAdminTableResultByCashier";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            CompanyID: companyid,
            Startdate: startdate,
            Enddate: enddate,
            Status: status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalResultsByAdmin = function (withdrawIDs, providerCode, cb) {
        var url = APIUrl + "/UpdateWithdrawalResultsByAdmin";
        var postData;

        postData = {
            BID: BID,
            WithdrawIDs: withdrawIDs,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalResultsByAdminCheck = function (withdrawIDs, status, userKey, cb) {
        var url = APIUrl + "/UpdateWithdrawalResultsByAdminCheck";
        var postData;

        postData = {
            BID: BID,
            WithdrawIDs: withdrawIDs,
            Status: status,
            UserKey: userKey
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalResultsByAdminDoubleCheck = function (withdrawIDs, userKey, cb) {
        var url = APIUrl + "/UpdateWithdrawalResultsByAdminDoubleCheck";
        var postData;

        postData = {
            BID: BID,
            WithdrawIDs: withdrawIDs,
            UserKey: userKey
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };
    //#endregion 

    //#region Currency
    this.getCurrency = function (cb) {
        var url = APIUrl + "/GetCurrency";

        callServiceByPost(url,BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getCurrencyByCompanyID = function (cb) {
        var url = APIUrl + "/GetCurrencyByCompanyID";

        callServiceByPost(url,BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertCurrency = function (currency, cb) {
        var url = APIUrl + "/InsertCurrency";

        //處理GetData
        //url = url + "?Currency=" + currency + "&BID=" + BID;
        var postData = {
            BID: BID,
            Currency: currency
        }
        callServiceByPost(url,postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getServiceType = function (companyID, cb) {
        var url = APIUrl + "/GetServiceType";
        //處理GetData
        //url = url + "?CompanyID=" + companyID + "&BID=" + BID;
        var postData = {
            BID: BID,
            CompanyID: companyID
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getServiceTypeAndProvideCode = function (cb) {
        var url = APIUrl + "/GetServiceTypeAndProvicerCode";
        //處理GetData

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getTestServiceType = function (cb) {
        var url = APIUrl + "/GetTestServiceType";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getTestServiceType2 = function (cb) {
        var url = APIUrl + "/GetTestServiceType2";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertServiceType = function (serviceTypeName, serviceType, currencyType, allowCollect, allowPay, serviceSupplyType, servicePaymentType, cb) {
        var url = APIUrl + "/InsertServiceType";

        //處理GetData
        //url = url + "?ServiceType=" + serviceType
        //    + "&CurrencyType=" + currencyType
        //    + "&AllowCollect=" + allowCollect
        //    + "&AllowPay=" + allowPay
        //    + "&ServiceTypeName=" + serviceTypeName
        //    + "&ServiceSupplyType=" + serviceSupplyType
        //    + "&ServicePaymentType=" + servicePaymentType + "&BID=" + BID;

        var postData = {
            BID: BID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            AllowCollect: allowCollect,
            AllowPay: allowPay,
            ServiceTypeName: serviceTypeName,
            ServiceSupplyType: serviceSupplyType,
            ServicePaymentType: servicePaymentType
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateServiceType = function (serviceTypeName, serviceType, currencyType, allowCollect, allowPay, serviceSupplyType, servicePaymentType, cb) {
        var url = APIUrl + "/UpdateServiceType";

        //處理GetData
        //url = url + "?ServiceType=" + serviceType
        //    + "&CurrencyType=" + currencyType
        //    + "&AllowCollect=" + allowCollect
        //    + "&AllowPay=" + allowPay
        //    + "&ServiceTypeName=" + serviceTypeName
        //    + "&ServiceSupplyType=" + serviceSupplyType
        //    + "&ServicePaymentType=" + servicePaymentType + "&BID=" + BID;

        var postData = {
            BID: BID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            AllowCollect: allowCollect,
            AllowPay: allowPay,
            ServiceTypeName: serviceTypeName,
            ServiceSupplyType: serviceSupplyType,
            ServicePaymentType: servicePaymentType
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    //#region Admin
    this.getAdminTableResult = function (companyID, cb) {
        var url = APIUrl + "/GetAdminTableResult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertAdmin = function (companyID, adminroleID, loginAccount, password, realName, description, adminType, cb) {
        var url = APIUrl + "/InsertAdmin";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            AdminroleID: adminroleID,
            LoginAccount: loginAccount,
            Password: password,
            RealName: realName,
            Description: description,
            AdminType: adminType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateAdmin = function (adminID, companyID, adminroleID, password, realName, description, adminType, adminState, cb) {
        var url = APIUrl + "/UpdateAdmin";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            AdminroleID: adminroleID,
            Password: password,
            RealName: realName,
            Description: description,
            AdminType: adminType,
            AdminState: adminState,
            AdminID: adminID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    this.updateLoginPassword = function (password, newpassword, cb) {
        var url = APIUrl + "/UpdateLoginPassword";
        var postData;

        postData = {
            BID: BID,
            Password: password,
            Newpassword: newpassword
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.disableAdmin = function (adminID, companyID, cb) {
        var url = APIUrl + "/DisableAdmin";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            AdminID: adminID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion


    //#region Admin


    this.getProxyProviderAcountResult = function (cb) {
        var url = APIUrl + "/GetProxyProviderAcountResult";
        var postData;

        postData = {
            BID: BID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProxyProviderRoleTableResult = function (cb) {
        var url = APIUrl + "/GetProxyProviderRoleTableResult";
        var postData;

        postData = {
            BID: BID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertProxyProviderAcount = function (adminroleID, loginAccount, password, realName, description, adminType, groupID, cb) {
        var url = APIUrl + "/InsertProxyProviderAcount";
        var postData;

        postData = {
            BID: BID,
            AdminroleID: adminroleID,
            LoginAccount: loginAccount,
            Password: password,
            RealName: realName,
            Description: description,
            AdminType: adminType,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateProxyProviderAcount = function (adminID, adminroleID, realName, description, adminType, adminState, groupID, cb) {
        var url = APIUrl + "/UpdateProxyProviderAcount";
        var postData;

        postData = {
            BID: BID,
            AdminroleID: adminroleID,
            RealName: realName,
            Description: description,
            AdminType: adminType,
            AdminState: adminState,
            AdminID: adminID,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.disableProxyProviderAcount = function (adminID, cb) {
        var url = APIUrl + "/DisableProxyProviderAcount";
        var postData;

        postData = {
            BID: BID,
            AdminID: adminID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    //#region BackendWithdrawalIP
    this.getBackendWithdrawalIPresult = function (companyID, cb) {
        var url = APIUrl + "/GetBackendWithdrawalIPresult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertBackendWithdrawalIP = function (companyID, IP, cb) {
        var url = APIUrl + "/InsertBackendWithdrawalIP";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            WithdrawalIP: IP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.deleteBackendWithdrawalIP = function (companyID, IP, cb) {
        var url = APIUrl + "/DeleteBackendWithdrawalIP";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            WithdrawalIP: IP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateBackendWithdrawalIPimg = function (imageData, imageName, IP, companyID, cb) {
        var url = APIUrl + "/UpdateBackendWithdrawalIPimg";
        var postData;

        postData = {
            BID: BID,
            ImageData: imageData,
            ImageName: imageName,
            WithdrawalIP: IP,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    //#region BackendIP
    this.getBackendIPresult = function (companyID, cb) {
        var url = APIUrl + "/GetBackendIPresult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertBackendIP = function (companyID, IP, cb) {
        var url = APIUrl + "/InsertBackendIP";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            WithdrawalIP: IP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.deleteBackendIP = function (companyID, IP, cb) {
        var url = APIUrl + "/DeleteBackendIP";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            WithdrawalIP: IP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateBackendIPimg = function (imageData, imageName, ip, companyID, type, imageID,cb) {
        var url = APIUrl + "/UpdateBackendIPimg";
        var postData;

        postData = {
            BID: BID,
            ImageData: imageData,
            ImageName: imageName,
            WithdrawalIP: ip,
            CompanyID: companyID,
            Type: type,
            ImageID: imageID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion



    //#region AdminRole
    this.getWithdrawalIPresult = function (companyID, cb) {
        var url = APIUrl + "/GetWithdrawalIPresult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertWithdrawalIP = function (companyID, IP, cb) {
        var url = APIUrl + "/InsertWithdrawalIP";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            WithdrawalIP: IP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.deleteWithdrawalIP = function (companyID, IP, cb) {
        var url = APIUrl + "/DeleteWithdrawalIP";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            WithdrawalIP: IP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.deleteImage = function (ImageID, cb) {
        var url = APIUrl + "/DeleteImageByImageID";

        var postData = {
            BID: BID,
            ImageID: ImageID
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateFrozenPointimg = function (imageData, imageName, frozenID, type, imageID, cb) {
        var url = APIUrl + "/UpdateFrozenPointimg";
        var postData;

        postData = {
            BID: BID,
            ImageData: imageData,
            ImageName: imageName,
            FrozenID: frozenID,
            Type: type,
            ImageID: imageID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateWithdrawalIPimg = function (imageData, imageName, IP, companyID, type, imageID, cb) {
        var url = APIUrl + "/UpdateWithdrawalIPimg";
        var postData;

        postData = {
            BID: BID,
            ImageData: imageData,
            ImageName: imageName,
            WithdrawalIP: IP,
            CompanyID: companyID,
            Type: type,
            ImageID: imageID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getAdminRoleTableResult = function (companyID, cb) {
        var url = APIUrl + "/GetAdminRoleTableResult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.InsertAdminRole = function (companyID, roleName, adminPermission, normalPermission, cb) {
        var url = APIUrl + "/InsertAdminRole";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            RoleName: roleName,
            AdminPermission: adminPermission,
            NormalPermission: normalPermission
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateAdminRole = function (companyID, adminRoleID, roleName, adminPermission, normalPermission, cb) {
        var url = APIUrl + "/UpdateAdminRole";
        var postData;

        postData = {
            BID: BID,
            AdminRoleID: adminRoleID,
            CompanyID: companyID,
            RoleName: roleName,
            AdminPermission: adminPermission,
            NormalPermission: normalPermission
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getPermissionByAdminRoleID = function (adminRoleID, cb) {
        var url = APIUrl + "/GetPermissionByAdminRoleID";
        var postData;

        postData = {
            BID: BID,
            AdminRoleID: adminRoleID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getPermissionTableResult = function (cb) {
        var url = APIUrl + "/GetPermissionTableResult";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };
    //#endregion

    //#region SummaryProviderByDate
    this.getSummaryProviderByDateTableResult = function (startDate, endDate, providerCode, serviceType, currencyType, cb) {
        var url = APIUrl + "/GetSummaryProviderByDateTableResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            ProviderCode: providerCode,
            ServiceType: serviceType,
            CurrencyType: currencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProxySummaryProviderByDateTableResult = function (startDate, endDate, groupID, cb) {
        var url = APIUrl + "/GetProxySummaryProviderByDateTableResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region PaymentTable

    this.getAbnormalPaymentTableResult = function (orderID, checkType, searchType, providercode, companyID, IP, cb) {
        var url = APIUrl + "/GetAbnormalPaymentTableResult";
        var postData;

        postData = {
            BID: BID,
            OrderID: orderID,
            CheckType: checkType,
            Providercode: providercode,
            CompanyID: companyID,
            SearchType: searchType,
            IP: IP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPaymentTableResultByAdmin = function (companyid, startDate, endDate, orderID, companyName, providerName, submitType, serviceType, processStatus, startAmount, endAmount, providerCode, cb) {
        var url = APIUrl + "/GetPaymentTableResultByAdmin";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyid,
            StartDate: startDate,
            EndDate: endDate,
            OrderID: orderID,
            CompanyName: companyName,
            ProviderName: providerName,
            SubmitType: submitType,
            ServiceType: serviceType,
            ProcessStatus: processStatus,
            StartAmount: startAmount,
            EndAmount: endAmount,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPaymentTableResultByLstPaymentID = function (lstPaymentID, cb) {
        var url = APIUrl + "/GetPaymentTableResultByLstPaymentID";
        var postData;

        postData = {
            BID: BID,
            PaymentIDs: lstPaymentID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPaymentProviderReportReviewResult = function (startDate, endDate, orderID, bankDescription, cb) {
        var url = APIUrl + "/GetPaymentProviderReportReviewResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            OrderID: orderID,
            PatchDescription: bankDescription
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPaymentReportReviewResult = function (companyid, startDate, endDate, orderID, processStatus, providerCode, serviceType, cb) {
        var url = APIUrl + "/GetPaymentReportReviewResult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyid,
            StartDate: startDate,
            EndDate: endDate,
            OrderID: orderID,
            ProcessStatus: processStatus,
            ProviderCode: providerCode,
            ServiceType: serviceType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderPaymentTableResultByWaitReview = function (processStatus, cb) {
        var url = APIUrl + "/GetProviderPaymentTableResultByWaitReview";
        var postData;

        postData = {
            BID: BID,
            ProcessStatus: processStatus
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPaymentTableResultByWaitReview = function (processStatus, cb) {
        var url = APIUrl + "/GetPaymentTableResultByWaitReview";
        var postData;

        postData = {
            BID: BID,
            ProcessStatus: processStatus
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPaymentTableResult = function (startDate, endDate, companyid, providercode, processStatus, paymentSerial, orderID, userIP, clientIP, cb) {
        var url = APIUrl + "/GetPaymentTableResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            CompanyID: companyid,
            ProviderCode: providercode,
            PaymentSerial: paymentSerial,
            OrderID: orderID,
            ProcessStatus: processStatus,
            UserIP: userIP,
            ClientIP: clientIP
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPaymentTransferLogResult = function (startDate, endDate, processStatus, paymentSerial, providerCode, cb) {
        var url = APIUrl + "/GetPaymentTransferLogResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            PaymentSerial: paymentSerial,
            ProcessStatus: processStatus,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getDownOrderTransferLogResult = function (startDate, endDate, processStatus, orderID, companyCode, isErrorOrder, cb) {
        var url = APIUrl + "/getDownOrderTransferLogResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            OrderID: orderID,
            ProcessStatus: processStatus,
            CompanyCode: companyCode,
            isErrorOrder: isErrorOrder
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    //交易紀錄
    this.getPaymentTableResult2 = function (startDate, endDate, paymentSerial, orderID, processStatus, cb) {
        var url = APIUrl + "/GetPaymentTableResult2";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            PaymentSerial: paymentSerial,
            OrderID: orderID,
            ProcessStatus: processStatus
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPatchPaymentTableResult = function (cb) {
        var url = APIUrl + "/GetPatchPaymentTableResult";
        var postData;

     

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeWithdrawalProcessStatus = function (transactionSerial, password, cb) {
        var url = APIUrl + "/ChangeWithdrawalProcessStatus";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: transactionSerial,
            Password: password

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.cancelWithdrawalReviewToSuccess = function (transactionSerial, password, cb) {
        var url = APIUrl + "/CancelWithdrawalReviewToSuccess";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: transactionSerial,
            Password: password

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.cancelWithdrawalReviewToFail = function (transactionSerial, password, cb) {
        var url = APIUrl + "/CancelWithdrawalReviewToFail";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: transactionSerial,
            Password: password

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.cancelWithdrawalProviderReview = function (transactionSerial, password, cb) {
        var url = APIUrl + "/CancelWithdrawalProviderReview";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: transactionSerial,
            Password: password

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeWithdrawalProcessStatusToSuccess = function (transactionSerial, password, cb) {
        var url = APIUrl + "/ChangeWithdrawalProcessStatusFailToSuccess";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: transactionSerial,
            Password: password

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changePaymentProcessStatus = function (transactionSerial, password, cb) {
        var url = APIUrl + "/ChangePaymentProcessStatus";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: transactionSerial,
            Password: password

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changePaymentProcessStatusSuccessToFail = function (transactionSerial, password, cb) {
        var url = APIUrl + "/ChangePaymentProcessStatusSuccessToFail";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: transactionSerial,
            Password: password

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getPatchPaymentTableResultByDate = function (startDate, endDate, cb) {
        var url = APIUrl + "/GetPatchPaymentTableResultByDate";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate

        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.savePaymentConfirm = function (paymentSerial, providerOrderID, patchAmount, password, cb) {
        var url = APIUrl + "/SavePaymentConfirm";
        var postData;

        postData = {
            BID: BID,
            PatchAmount: patchAmount,
            ProviderOrderID: providerOrderID,
            PaymentSerial: paymentSerial,
            Password: password
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.createPayment = function (amount, companyid, description, cb) {
        var url = APIUrl + "/CreatePayment";
        var postData;

        postData = {
            BID: BID,
            Amount: amount,
            CompanyID: companyid,
            Description: description,
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.createPatchPayment = function (patchDescription, amount, paymentSerial, cb) {
        var url = APIUrl + "/CreatePatchPayment";
        var postData;

        postData = {
            BID: BID,
            Amount: amount,
            PatchDescription: patchDescription,
            PaymentSerial: paymentSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.reSendPayment = function (paymentSerial, cb) {

        var url = APIUrl + "/ReSendPayment";

        var postData = {
            BID: BID,
            PaymentSerial: paymentSerial
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.PaymentRecord = function (paymentSerial, cb) {

        var url = APIUrl + "/PaymentRecord";

        var postData = {
            BID: BID,
            PaymentSerial: paymentSerial
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    //#endregion

    //#region BackendNotify 
    this.getBackendNotifyTableResult = function (cb) {
        var url = APIUrl + "/GetBackendNotifyTableResult";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getBackendNotifyTableResult2 = function (cb) {
        var url = APIUrl + "/GetBackendNotifyTableResult2";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region Permission
    this.getPermissionTableResultForPermissionSet = function (permissionCategoryID, cb) {
        var url = APIUrl + "/GetPermissionTableResultForPermissionSet";
        var postData;

        postData = {
            BID: BID,
            PermissionCategoryID: permissionCategoryID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertPermission = function (permissionName, adminPermission, description, linkURL, sortIndex, permissionCategoryID, cb) {
        var url = APIUrl + "/InsertPermission";
        var postData;

        postData = {
            BID: BID,
            PermissionName: permissionName,
            AdminPermission: adminPermission,
            Description: description,
            LinkURL: linkURL,
            PermissionCategoryID: permissionCategoryID,
            SortIndex: sortIndex
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updatePermission = function (permissionName, adminPermission, description, linkURL, sortIndex, permissionCategoryID, cb) {
        var url = APIUrl + "/UpdatePermission";
        var postData;

        postData = {
            BID: BID,
            PermissionName: permissionName,
            AdminPermission: adminPermission,
            Description: description,
            LinkURL: linkURL,
            PermissionCategoryID: permissionCategoryID,
            SortIndex: sortIndex
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.deletePermission = function (permissionName, cb) {
        var url = APIUrl + "/DeletePermission";
        var postData;

        postData = {
            BID: BID,
            PermissionName: permissionName
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getAdminRolePermissionResult = function (permissionName, cb) {
        var url = APIUrl + "/GetAdminRolePermissionResult";
        var postData;

        postData = {
            BID: BID,
            PermissionName: permissionName
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updatePermissionRole = function (permissionName, selectedAdminRole, cb) {
        var url = APIUrl + "/UpdatePermissionRole";
        var postData;

        postData = {
            BID: BID,
            PermissionName: permissionName,
            PermissionRoles: selectedAdminRole
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    //#region CompanyService
    this.insertCompanyServiceByEditView = function (companyID, serviceType, currencyType, collectRate, maxDaliyAmount, minOnceAmount, maxOnceAmount, state, checkoutType, cb) {
        var url = APIUrl + "/InsertCompanyServiceByEditView";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            CollectRate: collectRate,
            MaxDaliyAmount: maxDaliyAmount,
            MinOnceAmount: minOnceAmount,
            MaxOnceAmount: maxOnceAmount,
            State: state,
            CheckoutType: checkoutType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateCompanyServiceByEditView = function (companyID, serviceType, currencyType, collectRate, maxDaliyAmount, minOnceAmount, maxOnceAmount, state, checkoutType,cb) {
        var url = APIUrl + "/UpdateCompanyServiceByEditView";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            CollectRate: collectRate,
            MaxDaliyAmount: maxDaliyAmount,
            MinOnceAmount: minOnceAmount,
            MaxOnceAmount: maxOnceAmount,
            State: state,
            CheckoutType: checkoutType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateCompanyServiceWeightByEditView = function (companyID, serviceType, currencyType, lstProviderCode,  cb) {
        var url = APIUrl + "/UpdateCompanyServiceWeightByEditView";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            ProviderCodeAndWeight: lstProviderCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getCompanyServiceRelationByEditView = function (serviceType, providerCode,cb) {
        var url = APIUrl + "/GetCompanyServiceRelationByEditView";
        var postData;

        postData = {
            BID: BID,
            ServiceType: serviceType,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.setCompanyServiceRelationByEditView = function (companyID, serviceType, currencyType, providerCode, isAddRelation,  cb) {
        var url = APIUrl + "/SetCompanyServiceRelationByEditView";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            ProviderCode: providerCode,
            isAddRelation: isAddRelation
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getSelectedCompanyService = function (parentCompanyID, serviceType, currencyType, cb) {
        var url = APIUrl + "/GetSelectedCompanyService";
        //處理GetData
        //url = url + "?CompanyID=" + parentCompanyID
        //    + "&ServiceType=" + serviceType
        //    + "&CurrencyType=" + currencyType + "&BID=" + BID;


        var postData = {
            BID: BID,
            forCompanyID: parentCompanyID,
            ServiceType: serviceType,
            CurrencyType: currencyType
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };
    
    this.getCompanyServiceTableResult = function (companyID, cb) {
        var url = APIUrl + "/GetCompanyServiceTableResult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProviderServiceGPayRelationByCompany = function (serviceType, currencyType, forCompanyID, cb) {
        var url = APIUrl + "/GetProviderServiceGPayRelationByCompany";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: forCompanyID,
            CurrencyType: currencyType,
            ServiceType: serviceType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertCompanyService = function (companyID, serviceType, currencyType, collectRate, collectCharge, maxDaliyAmount, minOnceAmount, maxOnceAmount, checkoutType, state, lstProviderCode, description, cb) {
        var url = APIUrl + "/InsertCompanyService";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            CollectRate: collectRate,
            CollectCharge: collectCharge,
            MaxDaliyAmount: maxDaliyAmount,
            MinOnceAmount: minOnceAmount,
            MaxOnceAmount: maxOnceAmount,
            State: state,
            CheckoutType: checkoutType,
            ProviderCodeAndWeight: lstProviderCode,
            Description: description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateCompanyService = function (companyID, serviceType, currencyType, collectRate, collectCharge, maxDaliyAmount, minOnceAmount, maxOnceAmount, checkoutType, state, lstProviderCode, beforeMaxDaliyAmount, description, cb) {
        var url = APIUrl + "/UpdateCompanyService";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            CollectRate: collectRate,
            CollectCharge: collectCharge,
            MaxDaliyAmount: maxDaliyAmount,
            MinOnceAmount: minOnceAmount,
            MaxOnceAmount: maxOnceAmount,
            State: state,
            CheckoutType: checkoutType,
            ProviderCodeAndWeight: lstProviderCode,
            BeforeMaxDaliyAmount: beforeMaxDaliyAmount,
            Description: description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertCompanyService = function (companyID, serviceType, currencyType, collectRate, collectCharge, maxDaliyAmount, minOnceAmount, maxOnceAmount, checkoutType, state, lstProviderCode, description, cb) {
        var url = APIUrl + "/InsertCompanyService";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            CollectRate: collectRate,
            CollectCharge: collectCharge,
            MaxDaliyAmount: maxDaliyAmount,
            MinOnceAmount: minOnceAmount,
            MaxOnceAmount: maxOnceAmount,
            State: state,
            CheckoutType: checkoutType,
            ProviderCodeAndWeight: lstProviderCode,
            Description: description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateCompanyService = function (companyID, serviceType, currencyType, collectRate, collectCharge, maxDaliyAmount, minOnceAmount, maxOnceAmount, checkoutType, state, lstProviderCode, beforeMaxDaliyAmount, description, cb) {
        var url = APIUrl + "/UpdateCompanyService";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType,
            CollectRate: collectRate,
            CollectCharge: collectCharge,
            MaxDaliyAmount: maxDaliyAmount,
            MinOnceAmount: minOnceAmount,
            MaxOnceAmount: maxOnceAmount,
            State: state,
            CheckoutType: checkoutType,
            ProviderCodeAndWeight: lstProviderCode,
            BeforeMaxDaliyAmount: beforeMaxDaliyAmount,
            Description: description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    this.disableCompanyService = function (companyID, serviceType, currencyType, cb) {
        var url = APIUrl + "/DisableCompanyService";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyID,
            ServiceType: serviceType,
            CurrencyType: currencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    //#region Company
    this.getBankData = function (cb) {

        var url;
        url = APIUrl + "/GetBankData";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyTableResult = function (cb) {

        var url;
        url = APIUrl + "/GetCompanyTableResult";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.GetCompanyTableResultByCompanyType = function (cb) {

        var url;
        url = APIUrl + "/GetCompanyTableResultByCompanyType";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyTableResult2 = function (cb) {

        var url;
        url = APIUrl + "/GetCompanyTableResult2";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getAgentCompany = function (cb) {

        var url;
        url = APIUrl + "/GetAgentCompany";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getOffLineResult = function (cb) {

        var url;
        url = APIUrl + "/GetOffLineResult";

        var postData = {
            BID: BID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.getCompanyByID = function (cb) {

        var url;
        url = APIUrl + "/GetCompanyByID";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.fastCreateCompany = function (companyCode, cb) {
        var url = APIUrl + "/FastCreateCompany";
        var postData;

        postData = {
            BID: BID,
            CompanyCode: companyCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.insertCompanyTableResult = function (companyName, companyCode, URL, companyType, parentCompanyID, contacterName, contacterMobile, contacterMethod, contacterMethodAccount, contacterEmail, withdrawType, serviceType, backendLoginIPType, withdrawAPIType, backendWithdrawType, providerGroupID, currencyType, cb) {
        var url = APIUrl + "/InsertCompanyTableResult";
        var postData;

        postData = {
            BID: BID,
            CompanyName: companyName,
            CompanyCode: companyCode,
            URL: URL,
            CompanyType: companyType,
            ParentCompanyID: parentCompanyID == undefined ? 0 : parentCompanyID,
            ContacterName: contacterName,
            ContacterMobile: contacterMobile,
            ContacterMethod: contacterMethod,
            ContacterMethodAccount: contacterMethodAccount,
            ContacterEmail: contacterEmail,
            WithdrawType: withdrawType,
            AutoWithdrawalServiceType: serviceType,
            BackendLoginIPType: backendLoginIPType,
            WithdrawAPIType: withdrawAPIType,
            BackendWithdrawType: backendWithdrawType,
            ProviderGroupID: providerGroupID,
            CurrencyType: currencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updateCompanyTableResult = function (companyName, companyCode, URL, companyType, parentCompanyID, companyID, companyState, contacterName, contacterMobile, contacterMethod, contacterMethodAccount, contacterEmail, withdrawType, serviceType, checkCompanyWithdrawUrl, backendLoginIPType, withdrawAPIType, backendWithdrawType, providerGroups, checkCompanyWithdrawType, description,cb) {
        var url = APIUrl + "/UpdateCompanyTableResult";
        var postData;

        postData = {
            BID: BID,
            CompanyName: companyName,
            CompanyCode: companyCode,
            URL: URL,
            CompanyType: companyType,
            ParentCompanyID: parentCompanyID == undefined ? 0 : parentCompanyID,
            CompanyID: companyID,
            CompanyState: companyState,
            ContacterName: contacterName,
            ContacterMobile: contacterMobile,
            ContacterMethod: contacterMethod,
            ContacterMethodAccount: contacterMethodAccount,
            ContacterEmail: contacterEmail,
            WithdrawType: withdrawType,
            AutoWithdrawalServiceType: serviceType,
            CheckCompanyWithdrawUrl: checkCompanyWithdrawUrl,
            BackendLoginIPType: backendLoginIPType,
            WithdrawAPIType: withdrawAPIType,
            BackendWithdrawType: backendWithdrawType,
            ProviderGroups: providerGroups,
            CheckCompanyWithdrawType: checkCompanyWithdrawType,
            Description: description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.UpdateCompanyWithdrawlType = function (withdrawType, serviceType, backendLoginIPType, withdrawAPIType, backendWithdrawType, cb) {
        var url = APIUrl + "/UpdateCompanyWithdrawlType";
        var postData;

        postData = {
            BID: BID,
            WithdrawType: withdrawType,
            AutoWithdrawalServiceType: serviceType,
            BackendLoginIPType: backendLoginIPType,
            WithdrawAPIType: withdrawAPIType,
            BackendWithdrawType: backendWithdrawType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };




    this.disableCompanyByID = function (CompanyID, cb) {

        var url;
        //url = APIUrl + "/DisableCompanyByID?seleCompanyID=" + CompanyID + "&BID=" + BID;

        var postData = {
            BID: BID,
            CompanyID: CompanyID
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    }
    //#endregion

    //#region ProviderCode
    this.getProviderByServiceType = function (serviceType, cb) {
        var url = APIUrl + "/GetProviderByServiceType";
        var postData;

        postData = {
            BID: BID,
            ServiceType: serviceType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };    

    this.getProviderCodeResultByShowType = function (cb) {
        var url = APIUrl + "/GetProviderCodeResultByShowType";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderCodeResult = function (cb) {
        var url = APIUrl + "/GetProviderCodeResult";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderCodeResultByProxyProvider = function (cb) {
        var url = APIUrl + "/GetProviderCodeResultByProxyProvider";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    this.getAllProviderPoint2 = function (cb) {
        var url = APIUrl + "/GetAllProviderPoint2";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getAllProviderPoint = function (companyid, cb) {
        var url = APIUrl + "/GetAllProviderPoint";
        var postData = {
            BID: BID,
            CompanyID: companyid
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getAllCompanyServicePoint = function (cb) {
        var url = APIUrl + "/GetAllCompanyServicePoint";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeProviderGroupByAdmin = function (groupID,withdrawSerial, cb) {
        var url = APIUrl + "/ChangeProviderGroupByAdmin";
        var postData;

        postData = {
            BID: BID,
            OrderSerial: withdrawSerial,
            GroupID:groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeProviderGroupWithdrawalsByAdmin = function (groupID, withdrawSerials, cb) {
        var url = APIUrl + "/ChangeProviderGroupWithdrawalsByAdmin";
        var postData;

        postData = {
            BID: BID,
            Withdrawals: withdrawSerials,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.changeProviderGroup = function (groupID, withdrawSerial, cb) {
        var url = APIUrl + "/ChangeProviderGroup";
        var postData;

        postData = {
            BID: BID,
            GroupID: groupID,
            OrderSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderCodeResultByProviderAPIType = function (providerAPIType, cb) {
        var url = APIUrl + "/GetProviderCodeResultByProviderAPIType";

        var postData = {
            BID: BID,
            setAPIType: providerAPIType
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.setProxyProviderData = function (rate, charge, forProviderCode, maxWithdrawalAmount, cb) {
        var url = APIUrl + "/SetProxyProviderData";
        var postData;

        postData = {
            BID: BID,
            forProviderCode: forProviderCode,
            Rate: rate,
            Charge: charge,
            MaxWithdrawalAmount: maxWithdrawalAmount
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.confirmModifyBankCradByPayment = function (paymentserial, detail, cb) {
        var url = APIUrl + "/ConfirmModifyBankCradByPayment";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: paymentserial,
            PatchDescription: detail
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.confirmModifyBankCrad = function (withdrawSerial, bankDescription, cb) {
        var url = APIUrl + "/ConfirmModifyBankCrad";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial,
            BankDescription: bankDescription
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.checkHandleByAdmin = function (withdrawSerial, cb) {
        var url = APIUrl + "/CheckHandleByAdmin";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.cancelcheckHandleByAdmin = function (withdrawSerial, cb) {
        var url = APIUrl + "/CancelCheckHandleByAdmin";
        var postData;

        postData = {
            BID: BID,
            WithdrawSerial: withdrawSerial
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertProviderCodeResult = function (providerCode, providerName, introducer, providerUrl, merchantKey, merchantCode, notifySyncUrl, notifyAsyncUrl, providerAPIType, collectType, withdrawRate,companyID, cb) {
        var url = APIUrl + "/InsertProviderCodeResult";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providerCode,
            ProviderName: providerName,
            Introducer: introducer,
            ProviderUrl: providerUrl,
            MerchantKey: merchantKey,
            MerchantCode: merchantCode,
            NotifySyncUrl: notifySyncUrl,
            NotifyAsyncUrl: notifyAsyncUrl,
            CollectType: collectType,
            ProviderAPIType: providerAPIType,
            WithdrawRate: withdrawRate,
            forCompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updateProviderCodeResult = function (providerCode, providerName, introducer, providerUrl, merchantKey, merchantCode, notifySyncUrl, notifyAsyncUrl, providerAPIType, collectType, withdrawRate,companyID,cb) {
        var url = APIUrl + "/UpdateProviderCodeResult";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providerCode,
            ProviderName: providerName,
            Introducer: introducer,
            ProviderUrl: providerUrl,
            MerchantKey: merchantKey,
            MerchantCode: merchantCode,
            NotifySyncUrl: notifySyncUrl,
            NotifyAsyncUrl: notifyAsyncUrl,
            CollectType: collectType,
            ProviderAPIType: providerAPIType,
            WithdrawRate: withdrawRate,
            forCompanyID: companyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region ProviderService
    this.deleteProviderService = function (providercode, serviceType, currencyType, cb) {
        var url = APIUrl + "/DeleteProviderService";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providercode,
            ServiceType: serviceType,
            CurrencyType: currencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);
                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProviderServiceResult = function (providerCode, cb) {
        var url = APIUrl + "/GetProviderServiceResult";
        postData = {
            BID: BID,
            ProviderCode: providerCode
        };
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertProviderServiceResult = function (ProviderCode, ServiceType, CurrencyType, CostRate, CostCharge, MinOnceAmount, MaxOnceAmount, MaxDaliyAmount, CheckoutType, DeviceType, State, Description, cb) {
        var url = APIUrl + "/InsertProviderServiceResult";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: ProviderCode,
            ServiceType: ServiceType,
            CurrencyType: CurrencyType,
            CostRate: CostRate,
            CostCharge: CostCharge,
            MaxOnceAmount: MaxOnceAmount,
            MinOnceAmount: MinOnceAmount,
            MaxDaliyAmount: MaxDaliyAmount,
            CheckoutType: CheckoutType,
            DeviceType: DeviceType,
            State: State,
            Description: Description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updateProviderServiceResult = function (ProviderCode, ServiceType, CurrencyType, CostRate, CostCharge, MinOnceAmount, MaxOnceAmount, MaxDaliyAmount, CheckoutType, DeviceType, State, Description, cb) {
        var url = APIUrl + "/UpdateProviderServiceResult";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: ProviderCode,
            ServiceType: ServiceType,
            CurrencyType: CurrencyType,
            CostRate: CostRate,
            CostCharge: CostCharge,
            MaxOnceAmount: MaxOnceAmount,
            MinOnceAmount: MinOnceAmount,
            MaxDaliyAmount: MaxDaliyAmount,
            CheckoutType: CheckoutType,
            DeviceType: DeviceType,
            State: State,
            Description: Description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.disableProvider = function (ProviderCode, cb) {
        var url = APIUrl + "/DisableProvider";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: ProviderCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.disableProviderServiceResult = function (ProviderCode, ServiceType, CurrencyType, cb) {
        var url = APIUrl + "/DisableProviderServiceResult";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: ProviderCode,
            ServiceType: ServiceType,
            CurrencyType: CurrencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderServiceResult_Company = function (ServiceType, CurrencyType, cb) {
        var url = APIUrl + "/GetProviderServiceResult_Company";
        postData = {
            BID: BID,
            ServiceType: ServiceType,
            CurrencyType: CurrencyType
        };
        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region PermissionCategory
    this.getPermissionCategoryResult = function (cb) {
        var url = APIUrl + "/GetPermissionCategoryResult";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertPermissionCategoryResult = function (PermissionCategoryName, PageType, SortIndex, Description, cb) {
        var url = APIUrl + "/InsertPermissionCategoryResult";
        var postData;

        postData = {
            BID: BID,
            PermissionCategoryName: PermissionCategoryName,
            PageType: PageType,
            SortIndex: SortIndex,
            Description: Description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updatePermissionCategoryResult = function (PermissionCategoryID, PermissionCategoryName, PageType, SortIndex, Description, cb) {
        var url = APIUrl + "/UpdatePermissionCategoryResult";
        var postData;

        postData = {
            BID: BID,
            PermissionCategoryID: PermissionCategoryID,
            PermissionCategoryName: PermissionCategoryName,
            PageType: PageType,
            SortIndex: SortIndex,
            Description: Description
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.deletePermissionCategoryResult = function (PermissionCategoryID, cb) {
        var url = APIUrl + "/DeletePermissionCategoryResult";
        var postData;

        postData = {
            BID: BID,
            PermissionCategoryID: PermissionCategoryID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region AgentReceive
    this.getAgentReceiveTableResult = function (startDate, endDate, companyid, currencyType, cb) {

        var url = APIUrl + "/GetAgentReceiveTableResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            CompanyID: companyid,
            CurrencyType: currencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getAgentCloseTableResult = function (companyid, currencyType, cb) {

        var url = APIUrl + "/getAgentCloseTableResult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyid,
            CurrencyType: currencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.setAgentClose = function (cb) {

        var url = APIUrl + "/SetAgentClose";
        var postData = null;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.setAgentCloseByAdmin = function (companyID, cb) {

        var url = APIUrl + "/SetAgentCloseByAdmin";

        var postData = {
            BID: BID,
            CompanyID: companyID
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region CompanyPoint
    this.getCanUseCompanyServicePoint = function (cb) {

        var url = APIUrl + "/GetCanUseCompanyServicePoint";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCanUseCompanyServicePointByService = function (serviceType, currencyType, cb) {

        var url = APIUrl + "/GetCanUseCompanyServicePointByService";
        var postData = {
            BID: BID,
            ServiceType: serviceType,
            CurrencyType: currencyType
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyServicePointDetail = function (cb) {

        var url = APIUrl + "/GetCompanyServicePointDetail";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyServicePointDetail2 = function (companyID, cb) {

        var url = APIUrl + "/GetCompanyServicePointDetail2";

        var postData = {
            BID: BID,
            CompanyID: companyID
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyServicePointByServiceType = function (serviceType, cb) {

        var url = APIUrl + "/GetCompanyServicePointByServiceType";

        var postData = {
            BID: BID,
            ServiceType: ServiceType
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.getCompanyPointTableResult = function (seleCompanyID, cb) {

        var url = APIUrl + "/GetCompanyPointTableResult";

        var postData = {
            BID: BID,
            CompanyID: seleCompanyID
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.getCompanyPointAndCompanyServicePointResult = function (cb) {

        var url = APIUrl + "/GetCompanyPointAndCompanyServicePointResult";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.GetAgentPointResult = function (seleCompanyID, cb) {

        var url = APIUrl + "/GetAgentPointResult";

        var postData = {
            BID: BID,
            CompanyID: seleCompanyID
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertCompanyPointTableResult = function (seleCompanyID, CurrencyType, cb) {

        var url = APIUrl + "/InsertCompanyPointTableResult";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: seleCompanyID,
            CurrencyType: CurrencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updateCompanyPointTableResult = function (seleCompanyID, CurrencyType, PointValue, cb) {

        var url = APIUrl + "/UpdateCompanyPointTableResult";
        var postData;

        postData = {
            BID: BID,
            seleCompanyID: seleCompanyID,
            CurrencyType: CurrencyType,
            PointValue: PointValue
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region  GPayRelation
    
    this.getGPayRelationByCompany = function (forCompanyID, serviceType, currencyType, cb) {

        var url = APIUrl + "/GetGPayRelationByCompany";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: forCompanyID,
            ServiceType: serviceType,
            CurrencyType: currencyType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getGPayRelationTableResult2 = function (ServiceType, CurrencyType, forCompanyID, cb) {

        var url = APIUrl + "/GetGPayRelationTableResult2";
        var postData;

        postData = {
            BID: BID,
            ServiceType: ServiceType,
            CurrencyType: CurrencyType,
            forCompanyID: forCompanyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getGPayRelationTableResult = function (providercode, cb) {
        var url = APIUrl + "/GetGPayRelationTableResult";
        var postData;

        postData = {
            BID: BID,
            Providercode: providercode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getGPayRelationTableResultByServiceType = function (serviceType, cb) {
        var url = APIUrl + "/GetGPayRelationTableResultByServiceType";
        var postData;

        postData = {
            BID: BID,
            ServiceType: serviceType
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };
    //#endregion

    //#region  GPayWithdrawRelation
    this.getCompanyWithdrawRelationResult = function (companyid, cb) {
        var url = APIUrl + "/GetCompanyWithdrawRelationResult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: companyid
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };


    //#endregion

    //#region  SummaryCompanyByDate

    this.getCompanyServicePointHistoryResult = function (postdata, cb) {

        var url = APIUrl + "/GetCompanyServicePointHistoryResult";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByAgent = function (postdata, cb) {

        var url = APIUrl + "/GetSummaryCompanyByAgent";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByDateResultFlot = function (postdata, cb) {

        var url = APIUrl + "/GetSummaryCompanyByDateResultFlot";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByDateResult = function (postdata, cb) {

        var url = APIUrl + "/GetSummaryCompanyByDateResult";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByHourResult = function (postdata, cb) {

        var url = APIUrl + "/GetSummaryCompanyByHourResult";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByAgent2 = function (postdata,cb) {

        var url = APIUrl + "/GetSummaryCompanyByAgent2";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByAgentDownCompany = function (postdata, cb) {

        var url = APIUrl + "/GetSummaryCompanyByAgentDownCompany";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByDateResultByCurrencyType = function (postdata, cb) {

        var url = APIUrl + "/GetSummaryCompanyByDateResultByCurrencyType";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSummaryCompanyByDateResultForChart = function (postdata, cb) {

        var url = APIUrl + "/GetSummaryCompanyByDateResultForChart";
        postdata["BID"] = BID;
        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    //#endregion

    //#region  WithdrawLimit
    this.insertProviderWithdrawLimitResult = function (providerCode, currencyType, minLimit, maxLimit, charge, cb) {

        var postdata = {
            BID: BID,
            ProviderCode: providerCode,
            CurrencyType: currencyType,
            MinLimit: minLimit,
            MaxLimit: maxLimit,
            Charge: charge
        }

        var url = APIUrl + "/InsertProviderWithdrawLimitResult";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updateProviderWithdrawLimitResult = function (providerCode, currencyType, minLimit, maxLimit, charge, cb) {

        var postdata = {
            BID: BID,
            ProviderCode: providerCode,
            CurrencyType: currencyType,
            MinLimit: minLimit,
            MaxLimit: maxLimit,
            Charge: charge
        }

        var url = APIUrl + "/UpdateProviderWithdrawLimitResult";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertCompanyWithdrawLimitResult = function (companyid, currencyType, minLimit, maxLimit, charge, serviceType, cb) {

        var postdata = {
            BID: BID,
            CompanyID: companyid,
            CurrencyType: currencyType,
            MinLimit: minLimit,
            MaxLimit: maxLimit,
            Charge: charge,
            ServiceType: serviceType
        };

        var url = APIUrl + "/InsertCompanyWithdrawLimitResult";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updateCompanyWithdrawLimitResult = function (companyid, currencyType, minLimit, maxLimit, charge, serviceType, cb) {

        var postdata = {
            BID: BID,
            CompanyID: companyid,
            CurrencyType: currencyType,
            MinLimit: minLimit,
            MaxLimit: maxLimit,
            Charge: charge,
            ServiceType: serviceType
        }

        var url = APIUrl + "/UpdateCompanyWithdrawLimitResult";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getWithdrawLimitResult = function (companyid, providerCode, withdrawLimitType, cb) {

        var postdata = {
            BID: BID,
            CompanyID: companyid,
            ProviderCode: providerCode,
            WithdrawLimitType: withdrawLimitType
        }

        var url = APIUrl + "/GetWithdrawLimitResult";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getApiWithdrawLimit = function (companyid, cb) {

        var postdata = {
            BID: BID,
            CompanyID: companyid
        };

        var url = APIUrl + "/GetApiWithdrawLimit";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertGPayWithdrawRelation = function (companyID, charge, minLimit, maxLimit, lstProviderCode, cb) {

        var postdata = {
            BID: BID,
            CompanyID: companyID,
            ProviderCodeAndWeight: lstProviderCode,
            Charge: charge,
            MinLimit: minLimit,
            MaxLimit: maxLimit
        };

        var url = APIUrl + "/InsertGPayWithdrawRelation";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.updateGPayWithdrawRelation = function (companyID, charge, minLimit, maxLimit, lstProviderCode, currencyType, cb) {

        var postdata = {
            BID: BID,
            CompanyID: companyID,
            ProviderCodeAndWeight: lstProviderCode,
            Charge: charge,
            MinLimit: minLimit,
            MaxLimit: maxLimit,
            CurrencyType: currencyType
        };

        var url = APIUrl + "/UpdateGPayWithdrawRelation";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };


    this.getGPayWithdrawRelationByCompanyID = function (companyid, cb) {

        var postdata = {
            BID: BID,
            CompanyID: companyid
        };

        var url = APIUrl + "/GetGPayWithdrawRelationByCompanyID";

        callServiceByPost(url, postdata, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    //#endregion

    this.getProxyProviderPointHistory = function (startDate, endDate, operatorType, groupID, cb) {
        var url = APIUrl + "/GetProxyProviderPointHistory";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            OperatorType: operatorType,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getPermissionTableResultbyAdminID = function (cb) {
        var url = APIUrl + "/GetPermissionTableResultbyAdminID";

        //處理GetData

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#region 操作Log

    this.getAdminOPLogResult = function (startDate, endDate, type, companyid, cb) {
        var url = APIUrl + "/GetAdminOPLogResult";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            CompanyID: companyid,
            Type: type
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getAdminOPLogResultByCompany = function (startDate, endDate, cb) {
        var url = APIUrl + "/GetAdminOPLogResultByCompany";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region ManualHistory
    this.getProviderManualHistory = function (startDate, endDate, providercode, cb) {
        var url = APIUrl + "/GetProviderManualHistory";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            providercode: providercode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getProviderProfitManualHistoryResult = function (cb) {
        var url = APIUrl + "/GetProviderProfitManualHistoryResult";

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyManualHistory = function (startDate, endDate, company, servicetype, cb) {
        var url = APIUrl + "/GetCompanyManualHistory";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            forCompanyID: company,
            ServiceType: servicetype
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    

    this.getOrderByCompanyManualHistoryByFrozenPoint = function (transactionSerial, cb) {
        var url = APIUrl + "/GetOrderByCompanyManualHistoryByFrozenPoint";

        var postData = {
            BID: BID,
            TransactionSerial: transactionSerial
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getOrderByCompanyManualHistory = function (transactionSerial, cb) {
        var url = APIUrl + "/GetOrderByCompanyManualHistory";

        var postData = {
            BID: BID,
            TransactionSerial: transactionSerial
        }

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertProviderManualHistory = function (ProviderCode, CurrencyType, TransactionSerial, Amount, Description, Type, cb) {
        var url = APIUrl + "/InsertProviderManualHistory";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: ProviderCode,
            CurrencyType: CurrencyType,
            TransactionSerial: TransactionSerial,
            Amount: Amount,
            Description: Description,
            Type: Type
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertProviderManualHistoryByProfitAmount = function (ProviderCode, CurrencyType, TransactionSerial, Amount, Description, Type, isModifyProfit, cb) {
        var url = APIUrl + "/InsertProviderManualHistoryByProfitAmount";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: ProviderCode,
            CurrencyType: CurrencyType,
            TransactionSerial: TransactionSerial,
            Amount: Amount,
            Description: Description,
            Type: Type,
            isModifyProfit: isModifyProfit
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertCompanyManualHistory = function (Company, ServiceType, TransactionSerial, Amount, Description, Type, currency, cb) {
        var url = APIUrl + "/InsertCompanyManualHistory";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: Company,
            ServiceType: ServiceType,
            TransactionSerial: TransactionSerial,
            Amount: Amount,
            Description: Description,
            Type: Type,
            CurrencyType: currency
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.insertManualHistory = function (Company, ServiceType, TransactionSerial, Amount, Description, Type, currency, providerCode, cb) {
        var url = APIUrl + "/InsertManualHistory";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: Company,
            ServiceType: ServiceType,
            TransactionSerial: TransactionSerial,
            Amount: Amount,
            Description: Description,
            Type: Type,
            CurrencyType: currency,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region 案件冻结
    this.InsertFrozenPoint = function (PaymentSerial, CompanyID, ProviderCode, CompanyAmount, ProviderAmount, Description, CurrencyType, PaymentID, ServiceType, BankCard, BankCardName, BankName,CheckboxActualProviderFrozenAmount,cb) {
        var url = APIUrl + "/InsertFrozenPoint";
        var postData;

        postData = {
            BID: BID,
            forPaymentSerial: PaymentSerial,
            forCompanyID: CompanyID,
            forProviderCode: ProviderCode,
            CompanyFrozenAmount: CompanyAmount,
            ProviderFrozenAmount: ProviderAmount,
            Description: Description,
            CurrencyType: CurrencyType,
            forPaymentID: PaymentID,
            ServiceType: ServiceType,
            BankCard: BankCard,
            BankCardName: BankCardName,
            BankName: BankName,
            BoolActualProviderFrozenAmount: CheckboxActualProviderFrozenAmount
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.ThawPoint = function (FrozenID, cb) {
        var url = APIUrl + "/ThawPoint";
        var postData = {
            BID: BID,
            FrozenID: FrozenID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getSumFrozenPoint = function (providerCode,cb) {
        var url = APIUrl + "/GetSumFrozenPoint";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getCompanyFrozenPointHistory = function (PaymentSerial, Status, cb) {
        var url = APIUrl + "/GetCompanyFrozenPointHistory";
        var postData;

        postData = {
            BID: BID,
            PaymentSerial: PaymentSerial,
            Status: Status
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getFrozenPointHistory = function (startDate, endDate, PaymentSerial, CompanyID, providercode, Status, groupID,  cb) {
        var url = APIUrl + "/getFrozenPointHistory";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            ProviderCode: providercode,
            CompanyID: CompanyID,
            PaymentSerial: PaymentSerial,
            Status: Status,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.getFrozenPointHistoryByProxyProvider = function (startDate, endDate, PaymentSerial, Status, groupID, cb) {
        var url = APIUrl + "/GetFrozenPointHistoryByProxyProvider";
        var postData;

        postData = {
            BID: BID,
            StartDate: startDate,
            EndDate: endDate,
            PaymentSerial: PaymentSerial,
            Status: Status,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region 黑名單
    this.InsertBlackList = function (CompanyID, userIP, bankCard, bankCardName, cb) {
        var url = APIUrl + "/InsertBlackList";
        var postData;

        postData = {
            BID: BID,
            forCompanyID: CompanyID,
            UserIP: userIP,
            BankCard: bankCard,
            BankCardName: bankCardName
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.GetBlackListHistoryResult = function (CompanyID, userIP, bankCard, bankCardName, cb) {
        var url = APIUrl + "/GetBlackListHistoryResult";
        var postData;

        postData = {
            BID: BID,
            CompanyID: CompanyID,
            UserIP: userIP,
            BankCard: bankCard,
            BankCardName: bankCardName
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };

    this.CancelBlackList = function (BlackListID, cb) {
        var url = APIUrl + "/CancelBlackList";
        var postData = {
            BID: BID,
            BlackListID: BlackListID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb) {
                    cb(true, obj);
                }
            } else {
                if (cb) {
                    cb(false, text);
                }
            }
        });
    };
    //#endregion

    //#region ProxyProviderGroup
    this.getProxyProviderGroupNameByAdmin = function (cb) {
        var url = APIUrl + "/GetProxyProviderGroupNameByAdmin";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProxyProviderGroupName = function (cb) {
        var url = APIUrl + "/GetProxyProviderGroupName";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProxyProviderGroupFrozenPoint = function (providerCode, cb) {
        var url = APIUrl + "/GetProxyProviderGroupFrozenPoint";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProxyProviderGroupTableResultByAdmin = function (providerCode, cb) {
        var url = APIUrl + "/GetProxyProviderGroupTableResultByAdmin";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getAllProxyProviderGroupTableResultByAdmin = function ( cb) {
        var url = APIUrl + "/GetAllProxyProviderGroupTableResultByAdmin";
        var postData;

        postData = {
            BID: BID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProxyProviderGroupWeightByAdmin = function (providerCode, cb) {
        var url = APIUrl + "/GetProxyProviderGroupWeightByAdmin";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProxyProviderGroupOnWithdrawalAmountResultByAdmin = function (providerCode, cb) {
        var url = APIUrl + "/GetProxyProviderGroupOnWithdrawalAmountResultByAdmin";
        var postData;

        postData = {
            BID: BID,
            ProviderCode: providerCode
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getProxyProviderGroupTableResult = function (cb) {
        var url = APIUrl + "/GetProxyProviderGroupTableResult";


        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.insertProxyProviderGroup = function (groupName, minAmount, maxAmount, cb) {
        var url = APIUrl + "/InsertProxyProviderGroup";
        var postData;

        postData = {
            BID: BID,
            GroupName: groupName,
            MinAmount: minAmount,
            MaxAmount: maxAmount
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateProxyProviderGroupWeight = function (data,cb) {
        var url = APIUrl + "/UpdateProxyProviderGroupWeight";
        var postData;

        postData = {
            BID: BID,
            GroupData: data
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.updateProxyProviderGroup = function (groupName, groupID, state, minAmount, maxAmount, cb) {
        var url = APIUrl + "/UpdateProxyProviderGroup";
        var postData;

        postData = {
            BID: BID,
            GroupName: groupName,
            GroupID: groupID,
            State: state,
            MinAmount: minAmount,
            MaxAmount: maxAmount
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.disableProxyProviderGroup = function (groupID, cb) {
        var url = APIUrl + "/DisableProxyProviderGroup";
        var postData;

        postData = {
            BID: BID,
            GroupID: groupID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    //#endregion

    this.checkLoginIP = function (cb) {
        var url = APIUrl + "/CheckLoginIP";
        var postData;


        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getUserIP = function (cb) {
        var url = APIUrl + "/GetUserIP";
        var postData;


        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };
    this.getUserIP2 = function (cb) {
        var url = APIUrl + "/GetUserIP2";
        var postData;

        callServiceByPost(url, BID, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.getOnlineList = function (forCompanyID,cb) {
        var url = APIUrl + "/GetOnlineList";

        //處理GetData

        postData = {
            BID: BID,
            CompanyID: forCompanyID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    this.kickBackendUser = function (SessionID, cb) {
        var url = APIUrl + "/KickBackendUser";

        //處理GetData

        postData = {
            BID: BID,
            SessionID: SessionID
        };

        callServiceByPost(url, postData, function (success, text) {
            if (success == true) {
                var obj = getJSON(text);

                if (cb)
                    cb(true, obj);
            } else {
                if (cb)
                    cb(false, text);
            }
        });
    };

    function callServiceByGetSync(URL, cb) {
        var xmlHttp = new XMLHttpRequest;

        xmlHttp.open("GET", URL, false);
        xmlHttp.onreadystatechange = function () {
            if (this.readyState == 4) {
                var contentText = this.responseText;

                if (this.status == "200") {
                    if (cb) {
                        cb(true, contentText);
                    }
                } else {
                    cb(false, contentText);
                }
            }
        };

        xmlHttp.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        xmlHttp.send();
    }

    function callServiceByGet(URL, cb) {
        var xmlHttp = new XMLHttpRequest;

        xmlHttp.open("GET", URL, true);
        xmlHttp.onreadystatechange = function () {
            if (this.readyState == 4) {
                var contentText = this.responseText;

                if (this.status == "200") {
                    if (cb) {
                        cb(true, contentText);
                    }
                } else {
                    cb(false, "status:" + this.status + "," + contentText);
                }
            }
        };

        xmlHttp.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        xmlHttp.send();
    }

    function callServiceByPost(URL, postObject, cb) {
        var xmlHttp = new XMLHttpRequest;
        var postData;
        //let retry = 0;
        if (postObject)
            postData = JSON.stringify(postObject);

        xmlHttp.open("POST", URL, true);
        xmlHttp.onreadystatechange = function () {
            if (this.readyState == 4) {
                var contentText = this.responseText;

                if (this.status == "200") {
                    if (cb) {
                        cb(true, contentText);
                    }
                }
                //else if ((this.status == "400" || this.status == "408") && retry == 0) {
                //    xmlHttp.open("POST", URL, true);
                //    xmlHttp.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                //    xmlHttp.send(postData);
                //    retry = 1;
                //}
                else {
                    cb(false, "status:"+this.status+","+ contentText);
                }
            }
        };

        xmlHttp.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        xmlHttp.send(postData);
    }

    function getJSON(text) {
        var obj = JSON.parse(text);

        if (obj) {
            if (obj.hasOwnProperty('d')) {
                return obj.d;
            } else {
                return obj;
            }
        }
    }
}
