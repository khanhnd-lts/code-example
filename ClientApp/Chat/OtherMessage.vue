<template>
  <div class="other-message-body">
    <div v-if="profilePictureConfig.others" class="thum-container">
      <v-img
        class="participant-thumb grey"
        :src="
          message.creator && message.creator.profilePicture
            ? HOST +
              '/api/fileupload/download/' +
              message.creator.profilePicture
            : '/static/img/default-avatar.png'
        "
        :style="{
          width: profilePictureConfig.styles.width,
          height: profilePictureConfig.styles.height,
          'border-radius': profilePictureConfig.styles.borderRadius,
        }"
      ></v-img>
    </div>
    <div class="message-content">
      <template v-if="message.photo && message.photo != ''">
        <div v-if="message.loading" class="message-image">
          <v-img
            class="message-image-display img-overlay"
            :src="message.photo"
            @click="onImageClicked(message)"
          ></v-img>
          <div class="img-loading"></div>
        </div>
        <div v-else class="message-image">
          <v-img
            @click="onImageClicked(message)"
            class="message-image-display"
            :src="HOST + '/api/fileupload/download/' + message.photo"
          ></v-img>
        </div>
        <v-tooltip right v-if="message.content && message.content != ''">
          <template v-slot:activator="{ on, attrs }">
            <div
              class="message-text"
              v-bind="attrs"
              v-on="on"
              :style="{
                background: colors.message.myself.bg,
                color: colors.message.myself.text,
              }"
            >
              <!-- <p class="message-username">{{myself.name}}</p> -->
              <p>{{ message.content }}</p>
            </div>
          </template>
          <span>{{
            message.createdTime | moment(timestampConfig.format)
          }}</span>
        </v-tooltip>
      </template>
      <template v-else>
        <v-tooltip right>
          <template v-slot:activator="{ on, attrs }">
            <div
              class="message-text"
              v-bind="attrs"
              v-on="on"
              :style="{
                background: colors.message.others.bg,
                color: colors.message.others.text,
              }"
            >
              <!--<p class="message-username">{{message.creator.name}}</p>-->
              <p>{{ message.content }}</p>
            </div>
          </template>
          <span>{{
            message.createdTime | moment(timestampConfig.format)
          }}</span>
        </v-tooltip>
      </template>
      <!--<div class="message-timestamp" :style="{'justify-content': 'baseline'}">
        <template v-if="timestampConfig.relative">
            {{message.createdTime.toRelative()}}
        </template>
        <template v-else>
            {{message.createdTime | moment(timestampConfig.format)}}
        </template>
        <v-icon v-if="asyncMode && !message.loading && !message.viewed" :size="14" class="icon-sent">done</v-icon>
        <v-icon v-else-if="asyncMode && !message.loading && message.viewed" :size="14" class="icon-sent viewed">done_all</v-icon>
        <div v-else-if="asyncMode" class="message-loading"></div>
      </div>-->
    </div>
  </div>
</template>

<script lang="ts">
import { mapGetters, mapMutations } from "vuex";
import { Vue } from "vue-property-decorator";
export default Vue.extend({
  components: {},
  props: {
    message: {
      type: Object,
      required: true,
    },
    asyncMode: {
      type: Boolean,
      required: false,
      default: false,
    },
    colors: {
      type: Object,
      required: true,
    },
    /* onImageClicked: {
                type: Function,
                required: false,
                default: null
            }, */
    profilePictureConfig: {
      type: Object,
      required: true,
    },
    timestampConfig: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      HOST: process.env.VUE_APP_ROOT_API,
    };
  },
  computed: {},
  methods: {
    getParticipantById(participantId: any) {
      return {};
    },
    onImageClicked(message: any) {
      this.$emit("onImageClicked", message);
    },
  },
});
</script>

<style>
.container-message-display .other-message-body {
  display: flex;
  align-items: flex-end;
  padding-left: 10px;
  min-height: 37px;
}

.container-message-display .other-message-body .message-content {
  display: flex;
  align-items: flex-start;
  justify-content: flex-start;
  flex-direction: column;
  max-width: 70%;
}

.container-message-display .other-message-body .participant-thumb {
  /* width: 25px;
            height: 25px;
            border-radius: 50%; */
  margin-right: 10px;
}

.container-message-display .other-message-body .message-timestamp {
  padding: 2px 7px;
  border-radius: 15px;
  margin: 0;
  /* max-width: 50%; */
  overflow-wrap: break-word;
  text-align: left;
  font-size: 10px;
  color: #bdb8b8;
  width: 100%;
  display: flex;
  align-items: center;
}

.container-message-display .other-message-body .message-text {
  background: #fff;
  padding: 7px 12px 8px 12px;
  line-height: 1.33;
  border-radius: 18px;
  margin-top: 1px;
  max-width: 100%;
  overflow-wrap: break-word;
  text-align: left;
  white-space: pre-wrap;
  /* border-bottom-left-radius: 4px; */
  word-break: break-word;
}
</style>