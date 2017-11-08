﻿using Innofactor.SuomiFiIdentificationClient.Support;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Innofactor.SuomiFiIdentificationClient {

  public interface ISaml2ResponseValidator {
    Saml2AuthResponse Validate(string samlResponse, bool validateConditions);
  }

  public class Saml2ResponseValidator : ISaml2ResponseValidator {

    private readonly AuthStateAccessor authStateAccessor;
    private readonly SamlConfig config;
    private readonly ILogger<Saml2ResponseValidator> log;

    public Saml2ResponseValidator(AuthStateAccessor authStateAccessor, IOptions<SamlConfig> config, ILogger<Saml2ResponseValidator> log) {
      this.authStateAccessor = authStateAccessor;
      this.config = config.Value;
      this.log = log;
    }

    public bool IsValid(string samlResponse, bool validateConditions) {
      return Validate(samlResponse, validateConditions).Success;
    }

    public Saml2AuthResponse Validate(string samlResponse, bool validateConditions) {

      var authId = authStateAccessor.Id;

      if (string.IsNullOrEmpty(authId?.Value)) {
        log.LogInformation("SAML auth state was not initialized");
        return new Saml2AuthResponse(false);
      }

      var saml2Response = Saml2AuthResponse.Create(samlResponse, config.Saml2IdpCertificate, authId, config, validateConditions);

      return saml2Response;

    }

  }

}